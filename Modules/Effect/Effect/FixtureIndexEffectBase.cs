using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading;
using Vixen.Data.Value;
using Vixen.Sys;
using VixenModules.App.Curves;
using VixenModules.App.Fixture;
using VixenModules.Property.IntelligentFixture;

namespace VixenModules.Effect.Effect
{
	/// <summary>
	/// Intelligent fixture index effect base class.
	/// </summary>
	/// <typeparam name="T_Data">Type of effect module data</typeparam>
	public abstract class FixtureIndexEffectBase<T_Data> : FixtureEffectBase<T_Data>
	{
		#region Constructor

		/// <summary>
		/// Constructor
		/// </summary>
		/// <param name="helpURL">Help URL associated with the effect</param>
		public FixtureIndexEffectBase(string helpURL) : base(helpURL)
		{
			// Initialize the list of available index values
			IndexValues = new List<string>();
		}

		#endregion

		#region Public Properties

		/// <summary>
		/// List of all possible index names.
		/// This collection takes into account that the effect may have been applied to a group of hetrogenious fixtures.
		/// The indices from all fixtures that support the function are compiled into this collection.
		/// </summary>
		[Browsable(false)]
		public List<string> IndexValues { get; set; }

		#endregion-

		#region Protected Properties

		// <summary>
		/// Flag which indicates if the index supports a curve.
		/// </summary>
		protected bool IndexHasCurve { get; set; }

		/// <summary>
		/// Indicates if the node supports the effect's index function.
		/// </summary>
		protected bool SupportsIndexFunction { get; set; }

		#endregion

		#region Protected Methods

		/// <summary>
		/// Determines if the specified index function supports a curve.
		/// </summary>
		/// <param name="indexValue">Index value to analyze</param>
		/// <param name="functionName">Fixture function to analyze</param>
		/// <param name="functionIdentity">Expected fixture function identity</param>
		protected void UpdateSupportsCurve(string indexValue, string functionName, FunctionIdentity functionIdentity)
		{
			// Default the index value to NOT supporting a curve
			IndexHasCurve = false;

			// If the index value and function name are not empty then...
			if (!string.IsNullOrEmpty(indexValue) &&
				!string.IsNullOrEmpty(functionName))
			{
				// Get the function fixture index
				FixtureIndexBase fixtureIndex = GetFunctionIndex(indexValue, functionName, functionIdentity);

				// Set whether the index value supports a curve
				IndexHasCurve = fixtureIndex.UseCurve;
			}
		}

		/// <summary>
		/// Renders the specified fixture function and index value.
		/// </summary>
		/// <param name="fixtureFunctionName">Fixture function name</param>
		/// <param name="indexvalue">Function index value</param>
		/// <param name="indexCurve">Curve associated with the function and index</param>
		/// <param name="fixtureFunctionIdentity">Expected fixture function identity</param>
		/// <param name="cancellationToken">Indicator of the rendering has been cancelled</param>
		protected void PreRenderInternal(
			string fixtureFunctionName,
			string indexvalue,
			Curve indexCurve,
			FunctionIdentity fixtureFunctionIdentity,
			CancellationTokenSource cancellationToken = null)
		{
			// Get all the leaf nodes that support the index function and value
			IEnumerable<IElementNode> nodes = GetRenderNodesForIndexFunction(fixtureFunctionName, indexvalue);

			// Loop over the leaf nodes that support the function
			foreach (IElementNode node in nodes)
			{
				// Retrieve the fixture property associated with the node
				IntelligentFixtureModule fixtureProperty = GetIntelligentFixtureProperty(node);

				// Find the fixture function associated with the effect 
				FixtureFunction func = fixtureProperty.FixtureSpecification.GetInUseFunction(fixtureFunctionName, fixtureFunctionIdentity);

				// Find the index value				
				FixtureIndexBase fixtureIndex = func.GetIndexDataBase().Single(index => index.Name == indexvalue);

				// Render the index command
				RenderIndex(
					node,
					fixtureIndex,
					indexCurve,
					func,
					cancellationToken);
			}
		}

		/// <summary>
		///	Returns function index values that are compatible with the effect.
		///	This method gives derived effects the opportunity to omit some index values.
		/// </summary>
		/// <param name="function">Fixture function to analyze</param>
		/// <returns>Collection of fixture index values</returns>
		protected virtual List<FixtureIndexBase> GetCompatibleIndexValues(FixtureFunction function)
		{
			// By default return all index items
			return function.GetIndexDataBase();
		}

		/// <summary>
		/// Determines if the target nodes support the specified fixture function, index value and function identity.
		/// </summary>
		/// <param name="functionName">Fixture function name</param>
		/// <param name="indexValue">Fixture index value</param>
		/// <param name="functionIdentity">Fixture function identity</param>
		protected void UpdateFixtureCapabilities(ref string functionName, ref string indexValue, FunctionIdentity functionIdentity)
		{
			// Determine if the applicable nodes support the function
			DetermineFixtureCapabilities(ref functionName, ref indexValue, functionIdentity);

			// Update whether the index item supports a curve
			//UpdateSupportsCurve(indexValue, functionName, functionIdentity);
		}

		/// <summary>
		/// Determines if the target nodes support the specified fixture function, index value and function identity.
		/// </summary>
		/// <param name="functionName">Fixture function name</param>
		/// <param name="indexValue">Fixture index value</param>
		/// <param name="functionIdentity">Fixture function identity</param>
		protected void DetermineFixtureCapabilities(ref string functionName, ref string indexValue, FunctionIdentity functionIdentity)
		{
			// If there are element nodes associated with the effect
			if (TargetNodes.Any())
			{
				// Determine if any of the nodes support the function
				SupportsIndexFunction = GetFixtureFunctions().Any(func => func.FunctionIdentity == functionIdentity);
				
				// If the function name is NOT empty and
				// function is no longer associated with the node then...
				if (!string.IsNullOrEmpty(functionName) &&
					!GetMatchingFunctionNames(functionIdentity).Contains(functionName))
				{
					// Clear out the selected index function name
					functionName = String.Empty;
				}

				// If the index function name is empty and		
				// there are functions that support the function identity then...
				if (string.IsNullOrEmpty(functionName) &&
					GetMatchingFunctionNames(functionIdentity).Any())
				{
					// Initialize the selected function to the first function
					functionName = GetMatchingFunctionNames(functionIdentity).First();
				}

				
				// Clear the list of index values
				IndexValues.Clear();
				
				// Retrieve all leaf nodes that have an intelligent fixture property
				IEnumerable<IElementNode> leaves = GetLeafNodesWithIntelligentFixtureProperty(TargetNodes.First());
					
				// Loop over the leaves
				foreach (IElementNode leafNode in leaves)
				{
					// Retrieve the fixture property associated with the leaf node
					IntelligentFixtureModule fixtureProperty = GetIntelligentFixtureProperty(leafNode);					
												
					// If the function is mapped to a channel then...
					if (fixtureProperty.FixtureSpecification.IsFunctionUsed(functionName, functionIdentity))						
					{
						// Find the function associated with the effect
						FixtureFunction func = fixtureProperty.FixtureSpecification.GetInUseFunction(functionName, functionIdentity);

						// Loop over the function's index values
						foreach (string indexVal in GetCompatibleIndexValues(func).Select(val => val.Name))
						{
							// If the value is not already in this list then...
							if (!IndexValues.Contains(indexVal))
							{
								// Add the index value to the list
								IndexValues.Add(indexVal);
							}
						}
					}
				}

				
				// If an applicable function is associated with at least one node then...
				if (SupportsIndexFunction)
				{
					// Default the index value to the first index value
					if (string.IsNullOrEmpty(indexValue))
					{
						// Set the index value to the first index value
						indexValue = IndexValues[0];
					}

					// If the index value no longer exists in the collection then...
					if (!IndexValues.Contains(indexValue))
					{
						// Set the index value to the first index value from the collection
						indexValue = IndexValues[0];
					}
				}				
			}				
		}
		
		/// <summary>
		/// Returns the fixture function index item associated with the specified index value.
		/// </summary>
		/// <param name="indexValue">Index value to find</param>
		/// <param name="indexFunctionName">Fixture function name to search</param>
		/// <param name="functionIdentity">Expected identity of the function</param>
		/// <returns></returns>
		protected FixtureIndexBase GetFunctionIndex(string indexValue, string indexFunctionName, FunctionIdentity functionIdentity)
		{
			FixtureIndexBase fixtureIndex = null;

			// Retrieve all leaf nodes that have an intelligent fixture property
			IEnumerable<IElementNode> leaves = GetLeafNodesWithIntelligentFixtureProperty(TargetNodes.First());
			
			// Loop over the leaves
			foreach (IElementNode leafNode in leaves)
			{
				// Retrieve the fixture property associated with the leaf node
				IntelligentFixtureModule fixtureProperty = GetIntelligentFixtureProperty(leafNode);

				// Find the first function associated with the effect 
				FixtureFunction func = fixtureProperty.FixtureSpecification.GetInUseFunction(indexFunctionName, functionIdentity);
				
				// If a function was found then...
				if (func != null)
				{
					// Return the index value and break out of the loop
					fixtureIndex = func.GetIndexDataBase().Single(IndexVal => IndexVal.Name == indexValue);
					break;
				}
			}

			return fixtureIndex;
		}


		#endregion		
	}
}
