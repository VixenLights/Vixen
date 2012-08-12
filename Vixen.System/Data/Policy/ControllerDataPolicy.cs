namespace Vixen.Data.Policy {
	// This class exists to decouple the controllers from the exact data policy
	// class that needs to be implemented.
	abstract public class ControllerDataPolicy : DataFlowDataDispatchingDataPolicy {
	}
}
