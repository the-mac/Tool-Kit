using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using Microsoft.Phone.Controls;

namespace KeyboardInputScopeTestApp
{
    public partial class MainPage : PhoneApplicationPage
    {
        // Constructor
        public MainPage()
        {
            InitializeComponent();
            this.Loaded += new RoutedEventHandler(MainPage_Loaded);
        }

        private void MainPage_Loaded(object sender, RoutedEventArgs e)
        {
            InputScope scope = new InputScope();
            InputScopeName name = new InputScopeName();

            name.NameValue = InputScopeNameValue.Number;
            scope.Names.Add(name);

            txtSetByCode.InputScope = scope;
        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            System.Text.StringBuilder sb = new System.Text.StringBuilder();

            sb.AppendLine("What I know about you:");
            sb.AppendLine(txtName.Text);
            sb.AppendLine(txtCountry.Text);
            sb.AppendLine(txtNumber.Text);
            sb.AppendLine(txtEmail.Text);
            sb.AppendLine(txtWebsite.Text);
            sb.AppendLine(txtFriend.Text);
            sb.AppendLine(txtFeeling.Text);

            MessageBox.Show(sb.ToString());
        }
    }
}