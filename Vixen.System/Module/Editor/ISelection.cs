using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Vixen.Module.Editor {
	public interface ISelection {
		int DimensionCount { get; }
	}

	//Examples:
	//ISelection<int[]> - text selections with (start, length) character positions
	//ISelection<int[],Element[]> - grid selections with (start, length) and (selected elements)
	//ISelection<int[],int[],int[]> - volume with (start, length) of all three dimensions
	//ISelection<int[],int[],int[],int[]> - volume with (start, length) of all three dimensions and (start, length) of time
	
	public interface ISelection<Dim1Type> : ISelection {
		Dim1Type Bounds1 { get; set; }
	}

	public interface ISelection<Dim1Type, Dim2Type> : ISelection<Dim1Type> {
		Dim2Type Bounds2 { get; set; }
	}

	public interface ISelection<Dim1Type, Dim2Type, Dim3Type> : ISelection<Dim1Type, Dim2Type> {
		Dim3Type Bounds3 { get; set; }
	}

	public interface ISelection<Dim1Type, Dim2Type, Dim3Type, Dim4Type> : ISelection<Dim1Type, Dim2Type, Dim3Type> {
		Dim4Type Bounds4 { get; set; }
	}
}
