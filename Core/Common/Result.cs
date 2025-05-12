using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Common
{
	public class Result<T>
	{
		public bool IsSuccess { get; }
		public T Value { get; }
		public string Error { get; }

		protected Result(T value, bool isSuccess, string error)
		{
			Value = value;
			IsSuccess = isSuccess;
			Error = error;
		}

		public static Result<T> Success(T value) => new(value, true, null);
		public static Result<T> Failure(string error) => new(default, false, error);

		// Add for void results
		public static Result<bool> Success() => new(true, true, null);
	}
}
