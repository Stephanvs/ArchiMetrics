// --------------------------------------------------------------------------------------------------------------------
// <copyright file="SolutionProvider.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the SolutionProvider type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;

	using ArchiMetrics.Common;

	using Roslyn.Services;

	public class SolutionProvider : IProvider<string, ISolution>
	{
		private ConcurrentDictionary<string, ISolution> _cache = new ConcurrentDictionary<string, ISolution>();

		~SolutionProvider()
		{
			this.Dispose(false);
		}

		public ISolution Get(string path)
		{
			return this._cache.GetOrAdd(
				path,
				p =>
				{
					var workspace = Workspace.LoadSolution(p, "Release", "AnyCPU");
					return workspace.CurrentSolution;
				});
		}

		public IEnumerable<ISolution> GetAll(string key)
		{
			return from file in Directory.GetFiles(key, "*.sln", SearchOption.AllDirectories)
				   where this.IsValid(file)
				   let s = this.Get(file)
				   where s != null
				   select s;
		}

		public void Dispose()
		{
			this.Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool isDisposing)
		{
			if (isDisposing)
			{
				if (this._cache != null)
				{
					this._cache.Clear();
					this._cache = null;
				}
			}
		}

		private bool IsValid(string source)
		{
			return source.IndexOf("QuickStart", StringComparison.OrdinalIgnoreCase) == -1
			&& source.IndexOf("Demo", StringComparison.OrdinalIgnoreCase) == -1;
		}
	}
}
