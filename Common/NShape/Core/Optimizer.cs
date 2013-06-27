/******************************************************************************
  Copyright 2009-2012 dataweb GmbH
  This file is part of the NShape framework.
  NShape is free software: you can redistribute it and/or modify it under the 
  terms of the GNU General Public License as published by the Free Software 
  Foundation, either version 3 of the License, or (at your option) any later 
  version.
  NShape is distributed in the hope that it will be useful, but WITHOUT ANY
  WARRANTY; without even the implied warranty of MERCHANTABILITY or FITNESS FOR 
  A PARTICULAR PURPOSE.  See the GNU General Public License for more details.
  You should have received a copy of the GNU General Public License along with 
  NShape. If not, see <http://www.gnu.org/licenses/>.
******************************************************************************/

using System;


namespace Dataweb.Utilities
{

	#region Simulated Annealing

	/// <summary>
	/// Defines the problem for the simulated annealing algorithm.
	/// </summary>
	public interface ISimulatedAnnealingProblem
	{
		/// <ToBeCompleted></ToBeCompleted>
		object FindInitialState();

		/// <ToBeCompleted></ToBeCompleted>
		object FindNeighbourState(object state);

		/// <ToBeCompleted></ToBeCompleted>
		object SetState(object state);

		/// <ToBeCompleted></ToBeCompleted>
		float CalculateEnergy(object state);
	}


	/// <ToBeCompleted></ToBeCompleted>
	public class SimulatedAnnealingSolver
	{
		/// <summary>
		/// Initializes a new instance of <see cref="T:Dataweb.Utilities.SimulatedAnnealingSolver" />.
		/// </summary>
		public SimulatedAnnealingSolver()
		{
		}


		/// <ToBeCompleted></ToBeCompleted>
		public void Reset(ISimulatedAnnealingProblem problem, float maxEnergy)
		{
			this.problem = problem;
			this.maxEnergy = maxEnergy;
		}

		/// <ToBeCompleted></ToBeCompleted>
		public void Run()
		{
			object state = problem.FindInitialState();
			float energy = problem.CalculateEnergy(state);
			object bestState = state;
			float bestEnergy = energy;
			int loops = 0;
			while (loops < 1000 && energy > bestEnergy) {
				object nextState = problem.FindNeighbourState(state);
				float nextEnergy = problem.CalculateEnergy(nextState);
				if (nextEnergy < bestEnergy) {
					bestState = nextState;
					bestEnergy = nextEnergy;
				}
				if (AcceptanceProbability(energy, nextEnergy, Temperature(loops/1000)) > random.NextDouble()) {
					state = nextState;
					energy = nextEnergy;
				}
				++loops;
			}
			problem.SetState(bestState);
		}


		private float Temperature(float time)
		{
			return 1000*(1.0F - time);
		}


		private float AcceptanceProbability(float currentEnergy, float newEnergy, float temperature)
		{
			float result;
			if (newEnergy < currentEnergy) result = 1.0F;
			else if (temperature < 0.0001F) result = 0.0F;
			else result = (float) Math.Exp((currentEnergy - newEnergy)/temperature);
			return result;
		}


		private ISimulatedAnnealingProblem problem;

		private float maxEnergy;

		private Random random = new Random();
	}

	#endregion
}