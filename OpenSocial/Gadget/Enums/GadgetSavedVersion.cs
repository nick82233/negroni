using System;
using System.Collections.Generic;
using System.Text;

namespace MySpace.OpenSocial.Gadget
{
	/// <summary>
	/// Identifies a specific version of the gadget
	/// </summary>
	/// <remarks>
	/// Not happy with this design, tho functional.
	/// TODO: Fix to be two distinct paths: Published and Dev
	/// </remarks>
	public enum GadgetSavedVersion
	{
		/// <summary>
		/// Published source gadget
		/// </summary>
		Published = 0,
		/// <summary>
		/// Latest development gadget
		/// </summary>
		LatestDevelopment = 1,
		PreviousDev1 = 2,
		PreviousDev2 = 3,
		PreviousPublished1 = 4,
		PreviousPublished2 = 5
	}
}
