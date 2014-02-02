/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, http://code.msdn.microsoft.com/wpapps
  
*/
#include "pch.h"
#include "Compute.h"
#include "ppltasks.h"

using namespace ComputeComponent_Phone;
using namespace Platform;

using namespace concurrency;
using namespace Platform::Collections;
using namespace Windows::Foundation::Collections;
using namespace Windows::Foundation;
using namespace Windows::UI::Core;

//Public API
ComputeComponent::ComputeComponent()
{
}

// This method is deliberately simplistic. The goal is to demo
// implementation of a synchronous method. The calculation it performs 
// is for illustration purposes only.
int ComputeComponent::ComputeResult(int n1, int n2)
{
	return n1 + n2;
}

// This method is deliberately simplistic. The goal is to demo
// implementation of an asynchronous method. The calculation it performs 
// is for illustration purposes only. 
IAsyncOperation<int>^ ComputeComponent::ComputeResultAsync(int n1, int n2)
{
	 return create_async([this, n1, n2] () -> int {
		 return n1 + n2;
	 });
}

// This method computes all primes, orders them, then returns the results.
IAsyncOperationWithProgress<IVector<int>^, double>^ ComputeComponent::GetPrimesBulkAsync(int first, int last)
{
	return create_async([this, first, last] 
	(progress_reporter<double> reporter, cancellation_token cts) -> IVector<int>^ {

		// Ensure that the input values are in range.
		if (first < 0 || last < 0 || (last < first)) {
			throw ref new InvalidArgumentException();
		}

		Vector<int>^ primes = ref new Vector<int>();
		long range = last - first + 1;
		int percentInc = (range < 100) ? 1:range / 100;

		for (int n = first; n < last; n++) {

			if (cts.is_canceled())
			{
				reporter.report(0.0);
				cancel_current_task();
				return ref new Vector<int>();
			}

			if(n % percentInc == 0)
			{
				reporter.report(100.0 * n / range);
			}

			// If the value is prime, add it to the local vector.
			// This class defines two algorithms for determining whether a number is prime. 
			// By default, we are calling the slower version, since we are most interested
			// in showing a compute-intensive operation. Change this line to is_primeFast(n)
			// if you want an optimized calculation. 
			if (is_primeSlow(n)) {
				primes->Append(n);
			}
		};

		reporter.report(100.0);

		// Copy the results to a Vector object, which is 
		// implicitly converted to the IVector return type. IVector
		// makes collections of data available to other
		// Windows Runtime components.
		return primes;
	});
}

// This method returns no value. Instead, it raises an event each time a 
// prime is found, and passes the prime through the event.
IAsyncActionWithProgress<double>^ ComputeComponent::GetPrimesImmediateAsync(int first, int last)
{
	return create_async([this, first, last]
	(progress_reporter<double> reporter, cancellation_token cts)  {

		// Ensure that the input values are in range.
		if (first < 0 || last < 0) {
			throw ref new InvalidArgumentException();
		}

		// In this particular example, we don't actually use this to store 
		// results since we pass results one at a time directly back to 
		// UI as they are found. However, we have to provide this variable
		// as a parameter to parallel_for.
		Vector<int>^ primes;
		long range = last - first + 1;
		int percentInc = (range < 100) ? 1:range / 100;

		for (int n = first; n < last; n++) 
		{
			if (cts.is_canceled())
			{
				reporter.report(0.0);
				cancel_current_task();
			}

			if(n % percentInc == 0)
			{
				reporter.report(100.0 * n / range);
			}

			// If the value is prime, pass it immediately to the UI thread.
			if (is_primeSlow(n))
			{                
				this->primeFoundEvent(n);
			}
		}

		reporter.report(100.0);
	});
}



// Private Helpers
// Determines whether the input value is prime.
bool ComputeComponent::is_primeSlow(int n)
{
	// Brute Force
	if (n < 2)
		return false;
	for (int i = 2; i < n; ++i)
	{
		if ((n % i) == 0)
			return false;
	}
	return true;
}

// Private Helpers
// Determines whether the input value is prime.
bool ComputeComponent::is_primeFast(int n)
{
	if( n == 2 ) return true;
	if( n < 2 || n % 2 == 0 ) return false;
	long sqrt = sqrtl(n);

	for( int i=3; i<=sqrt; i+=2 ){   
		if(n % i == 0){ 
			return false;
		} 
	}
	return true;
}

