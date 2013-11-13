// --------------------------------------------------------------------------------------------------------------------
// <copyright file="NamespaceEdgeItemsRepository.cs" company="Reimers.dk">
//   Copyright � Reimers.dk 2013
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993 for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the NamespaceEdgeItemsRepository type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMetrics.UI.DataAccess
{
	using System;
	using System.Collections.Concurrent;
	using System.Collections.Generic;
	using System.IO;
	using System.Linq;
	using System.Threading.Tasks;
	using ArchiMetrics.Common;
	using ArchiMetrics.Common.CodeReview;
	using ArchiMetrics.Common.Metrics;
	using ArchiMetrics.Common.Structure;
	using Roslyn.Compilers.CSharp;
	using Roslyn.Services;

	public class NamespaceEdgeItemsRepository : CodeEdgeItemsRepository
	{
		private readonly ISolutionEdgeItemsRepositoryConfig _config;
		private readonly ConcurrentDictionary<string, IEnumerable<NamespaceReference>> _namespaceReferences = new ConcurrentDictionary<string, IEnumerable<NamespaceReference>>();
		private readonly IProvider<string, ISolution> _solutionProvider;

		public NamespaceEdgeItemsRepository(
			ISolutionEdgeItemsRepositoryConfig config,
			IProvider<string, ISolution> solutionProvider,
			ICodeErrorRepository codeErrorRepository)
			: base(config, codeErrorRepository)
		{
			_config = config;
			_solutionProvider = solutionProvider;
		}

		protected override async Task<IEnumerable<MetricsEdgeItem>> CreateEdges(IEnumerable<EvaluationResult> source)
		{
			var results = source.GroupBy(x => x.Namespace).ToArray();
			var namespaceReferences = await GetNamespaceReferences();
			return namespaceReferences
				.GroupBy(n => n.Namespace)
				.Where(g => g.Any())
				.Select(g => new NamespaceReference
				{
					Namespace = g.Key,
					References = g.SelectMany(n => n.References.Distinct().ToArray())
				})
				.SelectMany(r => r.References.Select((x, i) => CreateEdgeItem(r.Namespace, x, r.Namespace, new ProjectCodeMetrics(), new ProjectCodeMetrics(), results)))
				.ToArray();
		}

		private Task<IEnumerable<NamespaceReference>> GetNamespaceReferences()
		{
			return Task.Factory.StartNew(
				() => _namespaceReferences.GetOrAdd(
					_config.Path,
					path => _solutionProvider.Get(path)
						.Projects
						.Where(x =>
						{
							try
							{
								return x.HasDocuments;
							}
							catch
							{
								return false;
							}
						})
						.SelectMany(p => p.Documents)
						.Distinct(DocumentComparer.Default)
						.Select(d => d.GetSyntaxTree().GetRoot() as SyntaxNode)
						.Select(node => new Tuple<int, IEnumerable<string>, IEnumerable<string>>(GetLinesOfCode(node), GetNamespaceNames(node), GetUsings(node)))
						.SelectMany(t => t.Item2.Select(s => new NamespaceReference
						{
							Namespace = s,
							References = t.Item3.ToArray()
						}))
						.ToArray()
						.AsEnumerable()));
		}
	}
}
