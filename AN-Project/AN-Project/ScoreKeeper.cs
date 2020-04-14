using System;
using System.Collections.Generic;
using System.Text;

namespace AN_Project
{
    public class ScoreKeeper
    {
        private const double DEPTHWEIGHT = 0.80;
        private const double DEGREEWEIGHT = 0.10;
        private const double DISTANCEWEIGHT = 0.10;

        public double CurrentScore { get; private set; }

        public ScoreKeeper()
        {

        }

        /*
        public void CalculateTreeScore(OldState state)
        {
            double depthScore = CalculateDepthScore(state);
            //(double degreeScore, double distanceScore) = CalculateDegreeAndDistanceScore(state);

            CurrentScore = depthScore * DEPTHWEIGHT + degreeScore * DEGREEWEIGHT + distanceScore * DISTANCEWEIGHT;
        }
        //*/

        /*
        public void CalculateNodeScore(State state, Node node)
        {
            double newDegreeScore = Program.degreeRatios[node.Index] * node.depth;
            double newDistanceScore = Program.distanceRatiosToCOG[node.Index] * node.depth;

            double deltaDeg = newDegreeScore - node.DegreeScore;
            double deltaDist = newDistanceScore - node.DistanceScore;

            node.DegreeScore = newDegreeScore;
            node.DistanceScore = newDistanceScore;

            deltaDeg /= state.Tree.Nodes.Count;
            deltaDist /= state.Tree.Nodes.Count;

            CurrentScore += deltaDeg * DEGREEWEIGHT - deltaDist * DISTANCEWEIGHT; // TODO: We have also assumed this works for now :), but deep inside we know it won't work :'(((
        }
        //*/

        /*
        private double CalculateDepthScore(OldState state)
        {
            return 1 - (state.Tree.Depth - 2) / (double)state.Tree.Nodes.Count;
        }
        //*/

        /*
        private (double degree, double distance) CalculateDegreeAndDistanceScore(State state)
        {
            double totalDeg = 0;
            double totalDist = 0;

            foreach (Node node in state.Tree.Nodes)
            {
                node.DegreeScore = Program.degreeRatios[node.Index] * node.depth;
                node.DistanceScore = Program.distanceRatiosToCOG[node.Index] * node.depth;

                totalDeg += node.DegreeScore;
                totalDist += node.DistanceScore;
            }

            totalDeg /= state.Tree.Nodes.Count;
            totalDist /= state.Tree.Nodes.Count;

            double resDeg = Program.averageDegreeRatio - totalDeg;
            double resDist = 1 - (Program.averageDistanceRatio - totalDist); // TODO: We have assumed this works for now :)

            return (resDeg, resDist);
        }
        //*/
    }
}