﻿using System.Collections.Generic;
using System.Linq;
using ArchiCop.Core;

namespace ArchiCop.VisualStudioData
{
    public class VisualStudioProjectGraphEngine
    {
        public ArchiCopGraph<VisualStudioProject> GetGraph(IEnumerable<ArchiCopEdge<VisualStudioProject>> edges)
        {
            var graph = new ArchiCopGraph<VisualStudioProject>();
            graph.AddVerticesAndEdgeRange(edges);
            return graph;
        }

        public ArchiCopGraph<VisualStudioProject> GetGraph(IEnumerable<VisualStudioProject> projects)
        {
            var graph = new ArchiCopGraph<VisualStudioProject>();

            foreach (VisualStudioProject project in projects)
            {
                if (!graph.ContainsVertex(project))
                {
                    graph.AddVertex(project);
                }
            }

            foreach (VisualStudioProject projectFrom in projects)
            {
                foreach (VisualStudioProjectProjectReference projectReference in projectFrom.ProjectReferences)
                {
                    VisualStudioProject projectTo = projects.First(item => item.ProjectGuid == projectReference.Project);
                    graph.AddEdge(new ArchiCopEdge<VisualStudioProject>(projectFrom, projectTo));
                }
            }
            return graph;
        }
    }
}