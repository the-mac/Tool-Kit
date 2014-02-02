/* 
    Copyright (c) 2012 - 2013 Microsoft Corporation.  All rights reserved.
    Use of this sample source code is subject to the terms of the Microsoft license 
    agreement under which you licensed this sample source code and is provided AS-IS.
    If you did not accept the terms of the license agreement, you are not authorized 
    to use this sample source code.  For the terms of the license, please see the 
    license agreement between you and Microsoft.
  
    To see all Code Samples for Windows Phone, visit http://code.msdn.microsoft.com/wpapps
  
*/
#pragma once
#include <collection.h>
#include <math.h>


namespace ComputeComponent_Phone
{
	public delegate void PrimeFoundHandler(int result);

	public ref class ComputeComponent sealed
	{
	public:
		ComputeComponent();

		// Synchronous method. 
		int ComputeResult(int number1,int number2);

		// Asynchronous method
		Windows::Foundation::IAsyncOperation<int>^ ComputeResultAsync(int number1,int number2);

		// Asynchronous method that enables client code to receive updates on the progress of the operation.
		// Finds all prime numbers between first and last and then passes the results back to the caller.
		Windows::Foundation::IAsyncOperationWithProgress<Windows::Foundation::Collections::IVector<int>^, double>^
			GetPrimesBulkAsync(int first, int last);

		// Finds all prime numbers between first and last, passing each prime it finds to the callee via the primeFoundEvent.
		Windows::Foundation::IAsyncActionWithProgress<double>^ GetPrimesImmediateAsync(int first, int last);

		// Event
		event PrimeFoundHandler^ primeFoundEvent;

	private:
		// Helper method
		bool is_primeSlow(int n);
		bool is_primeFast(int n);
	};
}
