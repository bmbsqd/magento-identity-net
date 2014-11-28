using System;
using System.Threading.Tasks;

namespace Bmbsqd.Magento.Identity
{
	public static class TaskExtensions
	{
		public static async Task<TResult> Then<T, TResult>( this Task<T> task, Func<T, Task<TResult>> then )
		{
			var result = await task;
			return await then( result );
		}

		public static async Task Then<T>( this Task<T> task, Func<T, Task> then )
		{
			var result = await task;
			await then( result );
		}

		public static async Task Then( this Task task, Func<Task> then )
		{
			await task;
			await then();
		}

		public static async Task<TResult> Then<TResult>( this Task task, Func<Task<TResult>> then )
		{
			await task;
			return await then();
		}
	}
}