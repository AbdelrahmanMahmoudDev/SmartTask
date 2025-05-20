using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SmartTask.BL.Services
{
    public static class ConnectedNodes
    {
        public static void DFS(this HashSet<int> visited,int node, Dictionary<int, List<int>> graph)
        {
            if (!visited.Add(node)) return;

            if (graph.TryGetValue(node, out var neighbors))
            {
                foreach (var neighbor in neighbors)
                {
                    visited.DFS(neighbor, graph);
                }
            }
        }
    }
}
