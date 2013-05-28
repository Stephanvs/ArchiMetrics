// --------------------------------------------------------------------------------------------------------------------
// <copyright file="ProjectMetricsCalculatorTests.cs" company="Roche">
//   Copyright � Roche 2012
//   This source is subject to the Microsoft Public License (Ms-PL).
//   Please see http://go.microsoft.com/fwlink/?LinkID=131993] for details.
//   All other rights reserved.
// </copyright>
// <summary>
//   Defines the ProjectMetricsCalculatorTests type.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace ArchiMeter.CodeReview.Tests.Metrics
{
	using System.IO;
	using System.Linq;
	using CodeReview.Metrics;
	using NUnit.Framework;
	using Roslyn.Services;

	public sealed class ProjectMetricsCalculatorTests
	{
		private ProjectMetricsCalculatorTests()
		{
		}

		public class GivenAProjectMetricsCalculator
		{
			private ProjectMetricsCalculator _analyzer;

			[SetUp]
			public void Setup()
			{
				_analyzer = new ProjectMetricsCalculator();
			}

			[Test]
			public void CanCalculateMetricsForSilverlightProject()
			{
				var path = Path.GetFullPath(@"..\..\..\SampleSL\SampleSL.csproj");
				var workspace = Workspace.LoadStandAloneProject(path);
				var project = workspace.CurrentSolution.Projects.First();
				var task = _analyzer.Calculate(project);
				task.Wait();
				var metrics = task.Result.ToArray();

				Assert.IsNotEmpty(metrics);
			}

			[Test]
			public void WhenClassDefinitionIsEmptyThenHasCyclomaticComplexityOfOne()
			{
				const string text = @"namespace Testing
			{
				public class TestClass { }
			}";

				var task = _analyzer.Calculate(CreateProject(text));
				task.Wait();
				var metrics = task.Result;

				Assert.AreEqual(1, metrics.First().CyclomaticComplexity);
			}

			[Test]
			public void WhenClassDefinitionHasEmptyConstructorThenHasCyclomaticComplexityOfOne()
			{
				const string text = @"namespace Testing
			{
				public class TestClass {
	public TestClass(){}
}
			}";

				var task = _analyzer.Calculate(CreateProject(text));
				task.Wait();
				var metrics = task.Result;

				Assert.AreEqual(1, metrics.First().CyclomaticComplexity);
			}

			[TestCase(@"namespace Testing
			{
				using System;
using System.Linq;
			}", 0)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						const string x = ""blah"";
					}
				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						{}
					}
				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						var x = a + b;
					}
				}
			}", 2)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod() {
						foreach(var a in x){}
					}
				}
			}", 2)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
			: base(SyntaxWalkerDepth.Node)
		{
		}

				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public TestClass()
		{
		}

				}
			}", 1)]
			[TestCase(@"namespace Testing
			{
				public class TestClass {
					public void SomeMethod(SyntaxNode node)
					{
						base.VisitBinaryExpression(node);
						switch (node.Kind)
						{
							case SyntaxKind.LogicalNotExpression:
								_counter++;
								break;
							case SyntaxKind.ExclusiveOrExpression:
								break;
						}
					}
				}
			}", 4)]
			public void CodeHasExpectedLinesOfCode(string code, int loc)
			{
				var task = _analyzer.Calculate(CreateProject(code));
				task.Wait();
				var metrics = task.Result;

				Assert.AreEqual(loc, metrics.First().LinesOfCode);
			}

			//public void SomeMethod(SyntaxNode node)
			//{
			//	var x = 0;
			//	for (int i = 0; i < 10; i++)
			//	{
			//		if (i < 5)
			//		{
			//		}

			//		x += i;
			//	}
			//}

			private IProject CreateProject(string text)
			{
				ProjectId pid;
				DocumentId did;
				var solution = Solution.Create(SolutionId.CreateNewId("Metrics"))
									   .AddCSharpProject("testcode.dll", "testcode", out pid)
									   .AddDocument(pid, "TestClass.cs", text, out did)
									   .AddProjectReferences(pid, new ProjectId[0]);

				return solution.Projects.First();
			}
		}
	}
}