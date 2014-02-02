using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using System.IO.IsolatedStorage;
using System.IO;
using Microsoft.Xna.Framework.Input.Touch;
using System.Xml.Serialization;
using System.Xml;
using System.Xml.Schema;

namespace AlienGame
{
    /// <summary>
    /// This class encapsulates all of the gameplay screen's logic
    /// </summary>
    public class GameplayHelper : IXmlSerializable
    {
        private const string SerializationNamespace = @"http://schemas.microsoft.com/2003/10/Serialization/Arrays";

        //
        // Game Play Members
        //
        Rectangle worldBounds;
        bool gameOver;
        int baseLevelKillCount;
        int levelKillCount;
        float alienSpawnTimer;
        float alienSpawnRate;
        float alienMaxAccuracy;
        float alienSpeedMin;
        float alienSpeedMax;
        int alienScore;
        int nextLife;
        int hitStreak;
        int highScore;
        Random random;

        //
        // Rendering Members
        //
        Texture2D cloud1Texture;
        Texture2D cloud2Texture;
        Texture2D sunTexture;
        Texture2D moonTexture;
        Texture2D groundTexture;
        Texture2D tankTexture;
        Texture2D alienTexture;
        Texture2D badguy_blue;
        Texture2D badguy_red;
        Texture2D badguy_green;
        Texture2D badguy_orange;
        Texture2D mountainsTexture;
        Texture2D hillsTexture;
        Texture2D bulletTexture;
        Texture2D laserTexture;

        SpriteFont scoreFont;
        SpriteFont menuFont;

        Vector2 cloud1Position;
        Vector2 cloud2Position;
        Vector2 sunPosition;

        // Level changes, nighttime transitions, etc
        float transitionFactor; // 0.0f == day, 1.0f == night
        float transitionRate; // > 0.0f == day to night

        ParticleSystem particles;

        //
        // Audio Members
        //
        SoundEffect alienFired;
        SoundEffect alienDied;
        SoundEffect playerFired;
        SoundEffect playerDied;

        // Screen dimension constants
        const float screenHeight = 800.0f;
        const float screenWidth = 480.0f;
        const int leftOffset = 25;
        const int topOffset = 50;
        const int bottomOffset = 20;

        // Actor variables
        Player player;
        List<Alien> aliens;
        List<Bullet> alienBullets;
        List<Bullet> playerBullets;

        ContentManager contentManager;
        SpriteBatch spriteBatch;
        GraphicsDevice graphicsDevice;

        /// <summary>
        /// This constructor should only be used by the XML serializer.
        /// </summary>
        public GameplayHelper()
        {
            random = new Random();

            worldBounds = new Rectangle(0, 0, (int)screenWidth, (int)screenHeight);

            gameOver = true;

            player = new Player();
            playerBullets = new List<Bullet>();

            aliens = new List<Alien>();
            alienBullets = new List<Bullet>();

            particles = new ParticleSystem();
        }

        /// <summary>
        /// Creates a new instance of the game helper.
        /// </summary>
        /// <param name="contentManager">The content manager to use for loading the games content.</param>
        /// <param name="spriteBatch">The sprite batch to use for drawing the game's gameplay screen.</param>
        /// <param name="graphicsDevice">The graphics device on which the game is rendered.</param>
        public GameplayHelper(ContentManager contentManager, SpriteBatch spriteBatch,
            GraphicsDevice graphicsDevice)
        {
            random = new Random();

            worldBounds = new Rectangle(0, 0, (int)screenWidth, (int)screenHeight);

            gameOver = true;

            player = new Player();
            playerBullets = new List<Bullet>();

            aliens = new List<Alien>();
            alienBullets = new List<Bullet>();

            particles = new ParticleSystem(contentManager, spriteBatch);

            InitializeAssets(contentManager, spriteBatch, graphicsDevice);
        }

        /// <summary>
        /// Initializes the assets used by the gameplay helper.
        /// </summary>
        /// <param name="contentManager">The content manager to use for loading the games content.</param>
        /// <param name="spriteBatch">The sprite batch to use for drawing the game's gameplay screen.</param>
        /// <param name="graphicsDevice">The graphics device on which the game is rendered.</param>
        public void InitializeAssets(ContentManager contentManager, SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
        {
            this.contentManager = contentManager;
            this.spriteBatch = spriteBatch;
            this.graphicsDevice = graphicsDevice;

            SetAlienTextures();

            particles.InitializeAssets(contentManager, spriteBatch);
            particles.SetParticleTextures();
        }

        private void SetAlienTextures()
        {
            foreach (Alien alien in aliens)
            {
                alien.Texture = contentManager.Load<Texture2D>(alien.TextureName);
            }
        }

        #region Content loading/unloading

        /// <summary>
        /// Loads the content required for displaying the gameplay screen and starts the game.
        /// </summary>
        public void LoadContent()
        {
            cloud1Texture = contentManager.Load<Texture2D>("cloud1");
            cloud2Texture = contentManager.Load<Texture2D>("cloud2");
            sunTexture = contentManager.Load<Texture2D>("sun");
            moonTexture = contentManager.Load<Texture2D>("moon");
            groundTexture = contentManager.Load<Texture2D>("ground");
            tankTexture = contentManager.Load<Texture2D>("tank");
            mountainsTexture = contentManager.Load<Texture2D>("mountains_blurred");
            hillsTexture = contentManager.Load<Texture2D>("hills");
            alienTexture = contentManager.Load<Texture2D>("alien1");
            badguy_blue = contentManager.Load<Texture2D>("badguy_blue");
            badguy_red = contentManager.Load<Texture2D>("badguy_red");
            badguy_green = contentManager.Load<Texture2D>("badguy_green");
            badguy_orange = contentManager.Load<Texture2D>("badguy_orange");
            bulletTexture = contentManager.Load<Texture2D>("bullet");
            laserTexture = contentManager.Load<Texture2D>("laser");
            alienFired = contentManager.Load<SoundEffect>("Tank_Fire");
            alienDied = contentManager.Load<SoundEffect>("Alien_Hit");
            playerFired = contentManager.Load<SoundEffect>("Tank_Fire");
            playerDied = contentManager.Load<SoundEffect>("Player_Hit");
            scoreFont = contentManager.Load<SpriteFont>("ScoreFont");
            menuFont = contentManager.Load<SpriteFont>("MenuFont");

            LoadHighscore();
        }

        /// <summary>
        /// Unloads the game's particle system and saves the high-score.
        /// </summary>
        public void UnloadContent()
        {
            SaveHighscore();

            particles = null;
        }

        #endregion

        #region Update logic

        /// <summary>
        /// Updates the game's state.
        /// </summary>
        /// <param name="totalElapsedSeconds">The total seconds elapsed since the game started.</param>
        public void Update(float totalElapsedSeconds)
        {
            // Move the player
            if (player.IsAlive == true)
            {
                player.Position += player.Velocity * 128.0f * totalElapsedSeconds;
                player.FireTimer -= totalElapsedSeconds;

                if (player.Position.X <= 0.0f)
                    player.Position = new Vector2(0.0f, player.Position.Y);

                if (player.Position.X + player.Width >= worldBounds.Right)
                    player.Position = new Vector2(worldBounds.Right - player.Width, player.Position.Y);
            }

            Respawn(totalElapsedSeconds);

            UpdateAliens(totalElapsedSeconds);

            UpdateBullets(totalElapsedSeconds);

            CheckHits();

            if (player.IsAlive && player.Velocity.LengthSquared() > 0.0f)
                particles.CreatePlayerDust(player);

            particles.Update(totalElapsedSeconds);
        }

        /// <summary>
        /// Handles respawning the player if we are playing a game and the player is dead.
        /// </summary>
        /// <param name="elapsed">Time elapsed since Respawn was called last.</param>
        void Respawn(float elapsed)
        {
            if (gameOver)
                return;

            if (!player.IsAlive)
            {
                player.RespawnTimer -= elapsed;
                if (player.RespawnTimer <= 0.0f)
                {
                    // See if there are any bullets close...
                    int left = worldBounds.Width / 2 - tankTexture.Width / 2 - 8;
                    int right = worldBounds.Width / 2 + tankTexture.Width / 2 + 8;

                    for (int i = 0; i < alienBullets.Count; ++i)
                    {
                        if (alienBullets[i].IsAlive == false)
                            continue;

                        if (alienBullets[i].Position.X >= left || alienBullets[i].Position.X <= right)
                            return;
                    }

                    player.IsAlive = true;
                    player.Position = new Vector2(worldBounds.Width / 2 - player.Width / 2, worldBounds.Bottom - groundTexture.Height + 2 - player.Height);
                    player.Velocity = Vector2.Zero;
                    player.Lives--;
                }
            }
        }

        /// <summary>
        /// Moves all of the bullets (player and alien) and prunes "dead" bullets.
        /// </summary>
        /// <param name="elapsed"></param>
        void UpdateBullets(float elapsed)
        {
            for (int i = 0; i < playerBullets.Count; ++i)
            {
                if (playerBullets[i].IsAlive == false)
                    continue;

                playerBullets[i].Position += playerBullets[i].Velocity * elapsed;

                if (playerBullets[i].Position.Y < -32)
                {
                    playerBullets[i].IsAlive = false;
                    hitStreak = 0;
                }
            }

            for (int i = 0; i < alienBullets.Count; ++i)
            {
                if (alienBullets[i].IsAlive == false)
                    continue;

                alienBullets[i].Position += alienBullets[i].Velocity * elapsed;

                if (alienBullets[i].Position.Y > worldBounds.Height - groundTexture.Height - laserTexture.Height)
                    alienBullets[i].IsAlive = false;
            }
        }

        /// <summary>
        /// Moves the aliens and performs their "thinking" by determining if they
        /// should shoot and where.
        /// </summary>
        /// <param name="elapsed">The elapsed time since UpdateAliens was called last.</param>
        private void UpdateAliens(float elapsed)
        {
            // See if it's time to spawn an alien;
            alienSpawnTimer -= elapsed;
            if (alienSpawnTimer <= 0.0f)
            {
                SpawnAlien();
                alienSpawnTimer += alienSpawnRate;
            }

            for (int i = 0; i < aliens.Count; ++i)
            {
                if (aliens[i].IsAlive == false)
                    continue;

                aliens[i].Position += aliens[i].Velocity * elapsed;
                if ((aliens[i].Position.X < -aliens[i].Width - 64 && aliens[i].Velocity.X < 0.0f) ||
                    (aliens[i].Position.X > worldBounds.Width + 64 && aliens[i].Velocity.X > 0.0f))
                {
                    aliens[i].IsAlive = false;
                    continue;
                }

                aliens[i].FireTimer -= elapsed;

                if (aliens[i].FireTimer <= 0.0f && aliens[i].FireCount > 0)
                {
                    if (player.IsAlive)
                    {
                        Bullet bullet = CreateAlienBullet();
                        bullet.Position.X = aliens[i].Position.X + aliens[i].Width / 2 - laserTexture.Width / 2;
                        bullet.Position.Y = aliens[i].Position.Y + aliens[i].Height;
                        if ((float)random.NextDouble() <= aliens[i].Accuracy)
                        {
                            bullet.Velocity = Vector2.Normalize(player.Position - aliens[i].Position) * 64.0f;
                        }
                        else
                        {
                            bullet.Velocity = new Vector2(-8.0f + 16.0f * (float)random.NextDouble(), 64.0f);
                        }

                        alienFired.Play();
                    }

                    aliens[i].FireCount--;
                }
            }
        }

        /// <summary>
        /// Performs all bullet and player/alien collision detection.  Also handles game logic
        /// when a hit occurs, such as killing something, adding score, ending the game, etc.
        /// </summary>
        void CheckHits()
        {
            if (gameOver)
                return;

            for (int i = 0; i < playerBullets.Count; ++i)
            {
                if (playerBullets[i].IsAlive == false)
                    continue;

                for (int a = 0; a < aliens.Count; ++a)
                {
                    if (aliens[a].IsAlive == false)
                        continue;

                    if ((playerBullets[i].Position.X >= aliens[a].Position.X && playerBullets[i].Position.X <= aliens[a].Position.X + aliens[a].Width) && (playerBullets[i].Position.Y >= aliens[a].Position.Y && playerBullets[i].Position.Y <= aliens[a].Position.Y + aliens[a].Height))
                    {
                        playerBullets[i].IsAlive = false;
                        aliens[a].IsAlive = false;

                        hitStreak++;

                        player.Score += aliens[a].Score * (hitStreak / 5 + 1);

                        if (player.Score > highScore)
                            highScore = player.Score;

                        if (player.Score > nextLife)
                        {
                            player.Lives++;
                            nextLife += nextLife;
                        }

                        levelKillCount--;
                        if (levelKillCount <= 0)
                            AdvanceLevel();

                        particles.CreateAlienExplosion(new Vector2(aliens[a].Position.X + aliens[a].Width / 2, aliens[a].Position.Y + aliens[a].Height / 2));

                        alienDied.Play();
                    }
                }
            }

            if (player.IsAlive == false)
                return;

            for (int i = 0; i < alienBullets.Count; ++i)
            {
                if (alienBullets[i].IsAlive == false)
                    continue;

                if ((alienBullets[i].Position.X >= player.Position.X + 2 && alienBullets[i].Position.X <= player.Position.X + player.Width - 2) && (alienBullets[i].Position.Y >= player.Position.Y + 2 && alienBullets[i].Position.Y <= player.Position.Y + player.Height))
                {
                    alienBullets[i].IsAlive = false;

                    player.IsAlive = false;

                    hitStreak = 0;

                    player.RespawnTimer = 3.0f;
                    particles.CreatePlayerExplosion(new Vector2(player.Position.X + player.Width / 2, player.Position.Y + player.Height / 2));

                    playerDied.Play();

                    if (player.Lives <= 0)
                    {
                        gameOver = true;
                    }
                }

            }
        }

        /// <summary>
        /// Advances the difficulty of the game one level.
        /// </summary>
        void AdvanceLevel()
        {
            baseLevelKillCount += 5;
            levelKillCount = baseLevelKillCount;
            alienScore += 25;
            alienSpawnRate -= 0.3f;
            alienMaxAccuracy += 0.1f;
            if (alienMaxAccuracy > 0.75f)
                alienMaxAccuracy = 0.75f;

            alienSpeedMin *= 1.35f;
            alienSpeedMax *= 1.35f;

            if (alienSpawnRate < 0.33f)
                alienSpawnRate = 0.33f;

            if (transitionFactor == 1.0f)
            {
                transitionRate = -0.5f;
            }
            else
            {
                transitionRate = 0.5f;
            }
        }

        /// <summary>
        /// Returns an instance of a usable alien bullet.  Prefers reusing an existing (dead)
        /// bullet over creating a new instance.
        /// </summary>
        /// <returns>A bullet ready to place into the world.</returns>
        Bullet CreateAlienBullet()
        {
            Bullet b = null;

            for (int i = 0; i < alienBullets.Count; ++i)
            {
                if (alienBullets[i].IsAlive == false)
                {
                    b = alienBullets[i];
                    break;
                }
            }

            if (b == null)
            {
                b = new Bullet();
                alienBullets.Add(b);
            }

            b.IsAlive = true;

            return b;
        }

        /// <summary>
        /// Creates an instance of an alien, sets the initial state, and places it into the world.
        /// </summary>
        private void SpawnAlien()
        {
            Alien newAlien = CreateAlien();

            if (random.Next(2) == 1)
            {
                newAlien.Position.X = -64.0f;
                newAlien.Velocity.X = random.Next((int)alienSpeedMin, (int)alienSpeedMax);
            }
            else
            {
                newAlien.Position.X = worldBounds.Width + 32;
                newAlien.Velocity.X = -random.Next((int)alienSpeedMin, (int)alienSpeedMax);
            }

            newAlien.Position.Y = 24.0f + 80.0f * (float)random.NextDouble();

            // Aliens
            if (transitionFactor > 0.0f)
            {
                switch (random.Next(4))
                {
                    case 0:
                        newAlien.Texture = badguy_blue;
                        newAlien.TextureName = "badguy_blue";
                        break;
                    case 1:
                        newAlien.Texture = badguy_red;
                        newAlien.TextureName = "badguy_red";
                        break;
                    case 2:
                        newAlien.Texture = badguy_green;
                        newAlien.TextureName = "badguy_green";
                        break;
                    case 3:
                        newAlien.Texture = badguy_orange;
                        newAlien.TextureName = "badguy_orange";
                        break;
                }
            }
            else
            {
                newAlien.Texture = alienTexture;
                newAlien.TextureName = "alien1";
            }

            newAlien.Width = newAlien.Texture.Width;
            newAlien.Height = newAlien.Texture.Height;
            newAlien.IsAlive = true;
            newAlien.Score = alienScore;

            float duration = screenHeight / newAlien.Velocity.Length();

            newAlien.FireTimer = duration * (float)random.NextDouble();
            newAlien.FireCount = 1;

            newAlien.Accuracy = alienMaxAccuracy;
        }

        /// <summary>
        /// Returns an instance of a usable alien instance. Prefers reusing an existing (dead)
        /// alien over creating a new instance.
        /// </summary>
        /// <returns>An alien ready to place into the world.</returns>
        Alien CreateAlien()
        {
            Alien b = null;

            for (int i = 0; i < aliens.Count; ++i)
            {
                if (aliens[i].IsAlive == false)
                {
                    b = aliens[i];
                    break;
                }
            }

            if (b == null)
            {
                b = new Alien();
                aliens.Add(b);
            }

            b.IsAlive = true;

            return b;
        }

        /// <summary>
        /// Returns an instance of a usable player bullet.  Prefers reusing an /// existing (dead)
        /// bullet over creating a new instance.
        /// </summary>
        /// <returns>A bullet ready to place into the world.</returns>
        Bullet CreatePlayerBullet()
        {
            Bullet b = null;

            for (int i = 0; i < playerBullets.Count; ++i)
            {
                if (playerBullets[i].IsAlive == false)
                {
                    b = playerBullets[i];
                    break;
                }
            }

            if (b == null)
            {
                b = new Bullet();
                playerBullets.Add(b);
            }

            b.IsAlive = true;

            return b;
        }

        #endregion

        #region Drawing logic

        /// <summary>
        /// Draw the game's screen.
        /// </summary>
        /// <param name="totalElapsedSeconds">The total seconds elapsed since the game started.</param>
        public void Draw(float totalElapsedSeconds)
        {
            spriteBatch.Begin();

            DrawBackground(totalElapsedSeconds);
            DrawAliens();
            DrawPlayer();
            DrawBullets();
            particles.Draw();
            DrawForeground(totalElapsedSeconds);
            DrawHud();

            spriteBatch.End();
        }

        /// <summary>
        /// Draws the player's tank
        /// </summary>
        void DrawPlayer()
        {
            if (!gameOver && player.IsAlive)
            {
                spriteBatch.Draw(tankTexture, player.Position, Color.White);
            }
        }

        /// <summary>
        /// Draws all of the aliens.
        /// </summary>
        void DrawAliens()
        {
            for (int i = 0; i < aliens.Count; ++i)
            {
                if (aliens[i].IsAlive) spriteBatch.Draw(aliens[i].Texture,
                    new Rectangle((int)aliens[i].Position.X, (int)aliens[i].Position.Y, (int)aliens[i].Width, (int)aliens[i].Height), Color.White);
            }
        }

        /// <summary>
        /// Draw both the player and alien bullets.
        /// </summary>
        private void DrawBullets()
        {
            for (int i = 0; i < playerBullets.Count; ++i)
            {
                if (playerBullets[i].IsAlive)
                    spriteBatch.Draw(bulletTexture, playerBullets[i].Position, Color.White);
            }

            for (int i = 0; i < alienBullets.Count; ++i)
            {
                if (alienBullets[i].IsAlive)
                    spriteBatch.Draw(laserTexture, alienBullets[i].Position, Color.White);
            }
        }

        /// <summary>
        /// Draw the foreground, which is basically the clouds. I think I had planned on one point
        /// having foreground grass that was drawn in front of the tank.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since last Draw</param>
        private void DrawForeground(float elapsedTime)
        {
            // Move the clouds.  Movement seems like an Update thing to do, but this animations
            // have no impact over gameplay.
            cloud1Position += new Vector2(24.0f, 0.0f) * elapsedTime;
            if (cloud1Position.X > screenWidth)
                cloud1Position.X = -cloud1Texture.Width * 2.0f;

            cloud2Position += new Vector2(16.0f, 0.0f) * elapsedTime;
            if (cloud2Position.X > screenWidth)
                cloud2Position.X = -cloud1Texture.Width * 2.0f;

            spriteBatch.Draw(cloud1Texture, cloud1Position, Color.White);
            spriteBatch.Draw(cloud2Texture, cloud2Position, Color.White);
        }

        /// <summary>
        /// Draw the grass, hills, mountains, and sun/moon. Handle transitioning
        /// between day and night as well.
        /// </summary>
        /// <param name="elapsedTime">The elapsed time since last Draw</param>
        private void DrawBackground(float elapsedTime)
        {
            transitionFactor += transitionRate * elapsedTime;
            if (transitionFactor < 0.0f)
            {
                transitionFactor = 0.0f;
                transitionRate = 0.0f;
            }
            if (transitionFactor > 1.0f)
            {
                transitionFactor = 1.0f;
                transitionRate = 0.0f;
            }

            Vector3 day = Color.White.ToVector3();
            Vector3 night = new Color(80, 80, 180).ToVector3();
            Vector3 dayClear = Color.CornflowerBlue.ToVector3();
            Vector3 nightClear = night;

            Color clear = new Color(Vector3.Lerp(dayClear, nightClear, transitionFactor));
            Color tint = new Color(Vector3.Lerp(day, night, transitionFactor));

            // Clear the background, using the day/night color
            graphicsDevice.Clear(clear);

            // Draw the mountains
            spriteBatch.Draw(mountainsTexture, new Vector2(0, screenHeight - mountainsTexture.Height), tint);

            // Draw the hills
            spriteBatch.Draw(hillsTexture, new Vector2(0, screenHeight - hillsTexture.Height), tint);

            // Draw the ground
            spriteBatch.Draw(groundTexture, new Vector2(0, screenHeight - groundTexture.Height), tint);

            // Draw the sun or moon (based on time)
            spriteBatch.Draw(sunTexture, sunPosition, new Color(255, 255, 255, (byte)(255.0f * (1.0f - transitionFactor))));
            spriteBatch.Draw(moonTexture, sunPosition, new Color(255, 255, 255, (byte)(255.0f * transitionFactor)));
        }

        /// <summary>
        /// Draw the HUD, which consists of the score elements and the GAME OVER tag.
        /// </summary>
        void DrawHud()
        {
            float scale = 2.0f;

            if (gameOver)
            {
                Vector2 size = menuFont.MeasureString("GAME OVER");
                DrawString(menuFont, "GAME OVER", new Vector2(graphicsDevice.Viewport.Width / 2 - size.X, graphicsDevice.Viewport.Height / 2 - size.Y / 2), new Color(255, 64, 64), scale);

            }
            else
            {
                int bonus = 100 * (hitStreak / 5);
                string bonusString = (bonus > 0 ? " (" + bonus.ToString(System.Globalization.CultureInfo.CurrentCulture) + "%)" : "");
                // Score
                DrawString(scoreFont, "SCORE: " + player.Score.ToString(System.Globalization.CultureInfo.CurrentCulture) + bonusString, new Vector2(leftOffset, topOffset), Color.Yellow, scale);

                string text = "LIVES: " + player.Lives.ToString(System.Globalization.CultureInfo.CurrentCulture);
                Vector2 size = scoreFont.MeasureString(text);
                size *= scale;

                // Lives
                DrawString(scoreFont, text, new Vector2(screenWidth - leftOffset - (int)size.X, topOffset), Color.Yellow, scale);

                DrawString(scoreFont, "LEVEL: " + (((baseLevelKillCount - 5) / 5) + 1).ToString(System.Globalization.CultureInfo.CurrentCulture), new Vector2(leftOffset, screenHeight - bottomOffset), Color.Yellow, scale);

                text = "HIGH SCORE: " + highScore.ToString(System.Globalization.CultureInfo.CurrentCulture);

                size = scoreFont.MeasureString(text);

                DrawString(scoreFont, text, new Vector2(screenWidth - leftOffset - (int)size.X * 2, screenHeight - bottomOffset), Color.Yellow, scale);
            }
        }

        /// <summary>
        /// A simple helper to draw shadowed text.
        /// </summary>
        void DrawString(SpriteFont font, string text, Vector2 position, Color color)
        {
            spriteBatch.DrawString(font, text, new Vector2(position.X + 1, position.Y + 1), Color.Black);
            spriteBatch.DrawString(font, text, position, color);
        }

        /// <summary>
        /// A simple helper to draw shadowed text.
        /// </summary>
        void DrawString(SpriteFont font, string text, Vector2 position, Color color, float fontScale)
        {
            spriteBatch.DrawString(font, text, new Vector2(position.X + 1, position.Y + 1), Color.Black, 0, new Vector2(0, font.LineSpacing / 2), fontScale, SpriteEffects.None, 0);
            spriteBatch.DrawString(font, text, position, color, 0, new Vector2(0, font.LineSpacing / 2), fontScale, SpriteEffects.None, 0);
        }

        #endregion

        #region Input Handling

        /// <summary>
        /// Handles the user's input by updating the game appropriately.
        /// </summary>
        /// <param name="pauseGame">Whether or not the game is to be paused.</param>
        /// <param name="acceleration">A vector describing the phone's acceleration.</param>
        /// <param name="touchInfo">Touch information containing the user's touch input.</param>
        /// <returns>True if the game should end and false otherwise.</returns>
        public bool HandleInput(bool pauseGame, Vector3 acceleration, TouchCollection touchInfo)
        {
            if (pauseGame)
            {
                if (gameOver == true)
                {
                    return true;
                }
                else
                {
                    // There is no pause screen yet
                    return true;
                }
            }
            else
            {
                bool buttonTouched = false;

                //interpret touch screen presses
                foreach (TouchLocation location in touchInfo)
                {
                    switch (location.State)
                    {
                        case TouchLocationState.Pressed:
                            buttonTouched = true;
                            break;
                        case TouchLocationState.Moved:
                            break;
                        case TouchLocationState.Released:
                            break;
                    }
                }

                float movement = 0.0f;

                if (Math.Abs(acceleration.X) > 0.10f)
                {
                    if (acceleration.X > 0.0f)
                        movement = 1.0f;
                    else
                        movement = -1.0f;
                }

                player.Velocity.X = movement;

                if (buttonTouched)
                {
                    if (player.FireTimer <= 0.0f && player.IsAlive && !gameOver)
                    {
                        Bullet bullet = CreatePlayerBullet();
                        bullet.Position = new Vector2((int)(player.Position.X + player.Width / 2) - bulletTexture.Width / 2, player.Position.Y - 4);
                        bullet.Velocity = new Vector2(0, -256.0f);
                        player.FireTimer = 1.0f;

                        particles.CreatePlayerFireSmoke(player);
                        playerFired.Play();
                    }
                    else if (gameOver)
                    {
                        return true;
                    }
                }
            }

            return false;
        }

        #endregion

        #region Highscore loading/saving logic

        /// <summary>
        /// Saves the current high-score to a text file. The StorageDevice was selected during screen loading.
        /// </summary>
        private void SaveHighscore()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("highscores.txt", FileMode.Create, isf))
                {
                    using (StreamWriter writer = new StreamWriter(isfs))
                    {
                        writer.Write(highScore.ToString(System.Globalization.CultureInfo.InvariantCulture));
                        writer.Flush();
                        writer.Close();
                    }
                }
            }
        }

        /// <summary>
        /// Loads the high score from a text file.  The StorageDevice was selected during the loading screen.
        /// </summary>
        private void LoadHighscore()
        {
            using (IsolatedStorageFile isf = IsolatedStorageFile.GetUserStoreForApplication())
            {
                if (isf.FileExists("highscores.txt"))
                {
                    using (IsolatedStorageFileStream isfs = new IsolatedStorageFileStream("highscores.txt", FileMode.Open, isf))
                    {
                        using (StreamReader reader = new StreamReader(isfs))
                        {
                            try
                            {
                                highScore = Int32.Parse(reader.ReadToEnd(), System.Globalization.CultureInfo.InvariantCulture);
                            }
                            catch (FormatException)
                            {
                                highScore = 10000;
                            }
                            finally
                            {
                                if (reader != null)
                                    reader.Close();
                            }
                        }
                    }
                }
            }
        }

        #endregion

        /// <summary>
        /// Starts a new game session, setting all game states to initial values.
        /// </summary>
        public void Start()
        {
            cloud1Position = new Vector2(224 - cloud1Texture.Width, 32);
            cloud2Position = new Vector2(64, 80);

            sunPosition = new Vector2(16, 16);

            player.Width = tankTexture.Width;
            player.Height = tankTexture.Height;

            if (gameOver)
            {
                player.Score = 0;
                player.Lives = 3;
                player.RespawnTimer = 0.0f;

                gameOver = false;

                aliens.Clear();
                alienBullets.Clear();
                playerBullets.Clear();

                Respawn(0.0f);
            }

            transitionRate = 0.0f;
            transitionFactor = 0.0f;
            levelKillCount = 5;
            baseLevelKillCount = 5;
            alienScore = 25;
            alienSpawnRate = 1.0f;

            alienMaxAccuracy = 0.25f;

            alienSpeedMin = 24.0f;
            alienSpeedMax = 32.0f;

            alienSpawnRate = 2.0f;
            alienSpawnTimer = alienSpawnRate;

            nextLife = 5000;
        }

        #region IXmlSerializable Members & Helpers

        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Deserializes the game's state from a supplied XML reader.
        /// </summary>
        /// <param name="reader">The reader from which to deserialize game state data.</param>
        public void ReadXml(XmlReader reader)
        {
            // Read the wrapper element
            reader.Read();

            // deserialize basic gameplay elements
            DeserializeGameplayMembers(reader);

            // Deserialize the particle system
            XmlSerializer particleSystemSerializer = new XmlSerializer(typeof(ParticleSystem));
            particles = particleSystemSerializer.Deserialize(reader) as ParticleSystem;

            // Deserialize the player
            XmlSerializer playerSerializer = new XmlSerializer(typeof(Player));
            player = playerSerializer.Deserialize(reader) as Player;

            // Deserialize the various bullets
            XmlSerializer bulletSerializer = new XmlSerializer(typeof(Bullet));

            int playerBulletCount = int.Parse(reader.GetAttribute("Count"));

            // Read past the opening element for the player bullet list            
            reader.Read();

            for (int i = 0; i < playerBulletCount; i++)
            {
                playerBullets.Add(bulletSerializer.Deserialize(reader) as Bullet);
            }

            // Advance past the closing element if it exists
            if (playerBulletCount > 0)
            {
                reader.Read();
            }

            int alienBulletCount = int.Parse(reader.GetAttribute("Count"));

            // Read past the opening element for the alien bullet list
            reader.Read();

            for (int i = 0; i < alienBulletCount; i++)
            {
                alienBullets.Add(bulletSerializer.Deserialize(reader) as Bullet);
            }

            // Advance past the closing element if it exists
            if (alienBulletCount > 0)
            {
                reader.Read();
            }

            // Deserialize the aliens
            XmlSerializer alienSerializer = new XmlSerializer(typeof(Alien));

            int alienCount = int.Parse(reader.GetAttribute("Count"));

            // Read past the opening element for the alien list
            reader.Read();

            for (int i = 0; i < alienCount; i++)
            {
                aliens.Add(alienSerializer.Deserialize(reader) as Alien);
            }

            // Advance past the closing element if it exists
            if (alienCount > 0)
            {
                reader.Read();
            }

            reader.Read();
        }

        /// <summary>
        /// Deserializes all of the gameplay helper's simple members from an XML reader.
        /// </summary>
        /// <param name="writer">The reader to use for deserialization.</param>
        private void DeserializeGameplayMembers(XmlReader reader)
        {
            gameOver = reader.ReadElementContentAsBoolean("GameOver", SerializationNamespace);
            baseLevelKillCount = reader.ReadElementContentAsInt("BaseLevelKillCount", SerializationNamespace);
            levelKillCount = reader.ReadElementContentAsInt("LevelKillCount", SerializationNamespace);
            alienSpawnTimer = reader.ReadElementContentAsFloat("AlienSpawnTimer", SerializationNamespace);
            alienSpawnRate = reader.ReadElementContentAsFloat("AlienSpawnRate", SerializationNamespace);
            alienMaxAccuracy = reader.ReadElementContentAsFloat("AlienMaxAccuracy", SerializationNamespace);
            alienSpeedMin = reader.ReadElementContentAsFloat("AlienSpeedMin", SerializationNamespace);
            alienSpeedMax = reader.ReadElementContentAsFloat("AlienSpeedMax", SerializationNamespace);
            alienScore = reader.ReadElementContentAsInt("AlienScore", SerializationNamespace);
            nextLife = reader.ReadElementContentAsInt("NextLife", SerializationNamespace);
            hitStreak = reader.ReadElementContentAsInt("HitStreak", SerializationNamespace);
            highScore = reader.ReadElementContentAsInt("HighScore", SerializationNamespace);

            transitionFactor = reader.ReadElementContentAsFloat("TransitionFactor", SerializationNamespace);
            transitionRate = reader.ReadElementContentAsFloat("TransitionRate", SerializationNamespace);

            cloud1Position = DeserializeVector(reader);
            cloud2Position = DeserializeVector(reader);
            sunPosition = DeserializeVector(reader);
        }

        /// <summary>
        /// Serializes the gameplay helper, essentially the game's entire state, into the supplied XML writer.
        /// </summary>
        /// <param name="writer">The XML writer to perform the serialization with.</param>
        public void WriteXml(XmlWriter writer)
        {
            // Serialize basic gameplay elements
            SerializeGameplayMembers(writer);

            // Serialize the particle system
            XmlSerializer particleSystemSerializer = new XmlSerializer(typeof(ParticleSystem));
            particleSystemSerializer.Serialize(writer, particles);

            // Serialize the player
            XmlSerializer playerSerializer = new XmlSerializer(typeof(Player));
            playerSerializer.Serialize(writer, player);

            // Serialize the various bullets
            XmlSerializer bulletSerializer = new XmlSerializer(typeof(Bullet));

            writer.WriteStartElement("PlayerBullets");

            writer.WriteAttributeString("Count", playerBullets.Count.ToString());

            foreach (Bullet bullet in playerBullets)
            {
                bulletSerializer.Serialize(writer, bullet);
            }

            writer.WriteEndElement();

            writer.WriteStartElement("AlienBullets");

            writer.WriteAttributeString("Count", alienBullets.Count.ToString());

            foreach (Bullet bullet in alienBullets)
            {
                bulletSerializer.Serialize(writer, bullet);
            }

            writer.WriteEndElement();

            // Serialize the aliens
            XmlSerializer alienSerializer = new XmlSerializer(typeof(Alien));

            writer.WriteStartElement("Aliens");

            writer.WriteAttributeString("Count", aliens.Count.ToString());

            foreach (Alien alien in aliens)
            {
                alienSerializer.Serialize(writer, alien);
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Serializes all of the gameplay helper's simple members into an XML writer.
        /// </summary>
        /// <param name="writer">The writer to use for serialization.</param>
        private void SerializeGameplayMembers(XmlWriter writer)
        {
            writer.WriteElementString("GameOver", gameOver.ToString().ToLowerInvariant());
            writer.WriteElementString("BaseLevelKillCount", baseLevelKillCount.ToString());
            writer.WriteElementString("LevelKillCount", levelKillCount.ToString());
            writer.WriteElementString("AlienSpawnTimer", alienSpawnTimer.ToString());
            writer.WriteElementString("AlienSpawnRate", alienSpawnRate.ToString());
            writer.WriteElementString("AlienMaxAccuracy", alienMaxAccuracy.ToString());
            writer.WriteElementString("AlienSpeedMin", alienSpeedMin.ToString());
            writer.WriteElementString("AlienSpeedMax", alienSpeedMax.ToString());
            writer.WriteElementString("AlienScore", alienScore.ToString());
            writer.WriteElementString("NextLife", nextLife.ToString());
            writer.WriteElementString("HitStreak", hitStreak.ToString());
            writer.WriteElementString("HighScore", highScore.ToString());

            writer.WriteElementString("TransitionFactor", transitionFactor.ToString());
            writer.WriteElementString("TransitionRate", transitionRate.ToString());

            SerializeVector(writer, "Cloud1Position", cloud1Position);
            SerializeVector(writer, "Cloud2Position", cloud2Position);
            SerializeVector(writer, "SunPosition", sunPosition);
        }

        /// <summary>
        /// Serializes a Vector2 into an XML writer.
        /// </summary>
        /// <param name="writer">The XML writer to serialize the vector into.</param>
        /// <param name="elementName">The element name to give the element representing the vector.</param>
        /// <param name="vector">The vector to serialize.</param>
        private void SerializeVector(XmlWriter writer, string elementName, Vector2 vector)
        {
            writer.WriteStartElement(elementName);
            writer.WriteAttributeString("X", vector.X.ToString());
            writer.WriteAttributeString("Y", vector.Y.ToString());
            writer.WriteEndElement();
        }

        /// <summary>
        /// Deserializes the current element as a vector and then reads the next element.
        /// </summary>
        /// <param name="reader">The reader from which to deserialize the vector.</param>
        /// <returns>The vector contained in the node read from the XML reader.</returns>
        private Vector2 DeserializeVector(XmlReader reader)
        {
            Vector2 result = new Vector2(float.Parse(reader.GetAttribute("X")), float.Parse(reader.GetAttribute("Y")));
            reader.Read();
            return result;
        }

        #endregion
    }

    /// <summary>
    /// Represents either an alien or player bullet
    /// </summary>
    public class Bullet
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public bool IsAlive;
    }

    /// <summary>
    /// The player's state
    /// </summary>
    public class Player
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public float Width;
        public float Height;
        public bool IsAlive;
        public float FireTimer;
        public float RespawnTimer;
        public string Name;
        public int Score;
        public int Lives;
    }

    /// <summary>
    /// Data for an alien.  The only difference between the ships
    /// and the bad guys are the texture used.
    /// </summary>
    public class Alien
    {
        public Vector2 Position;
        [XmlIgnore]
        public Texture2D Texture;
        public string TextureName;
        public Vector2 Velocity;
        public float Width;
        public float Height;
        public int Score;
        public bool IsAlive;
        public float FireTimer;
        public float Accuracy;
        public int FireCount;
    }
}
