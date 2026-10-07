---
name: gortex-dynamics-joints-9-dirs
description: "Work in the Dynamics/Joints +9 dirs area — 658 symbols across 92 files (89% cohesion)"
---

# Dynamics/Joints +9 dirs

658 symbols | 92 files | 89% cohesion

## When to Use

Use this skill when working on files in:
- `src/Vixen.Common/Box2D/Collision/Shapes/b2ChainShape.cpp`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2ChainShape.h`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2CircleShape.cpp`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2CircleShape.h`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2EdgeShape.cpp`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2EdgeShape.h`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2PolygonShape.cpp`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2PolygonShape.h`
- `src/Vixen.Common/Box2D/Collision/Shapes/b2Shape.h`
- `src/Vixen.Common/Box2D/Collision/b2BroadPhase.cpp`
- `src/Vixen.Common/Box2D/Collision/b2BroadPhase.h`
- `src/Vixen.Common/Box2D/Collision/b2CollideCircle.cpp`
- `src/Vixen.Common/Box2D/Collision/b2CollideEdge.cpp`
- `src/Vixen.Common/Box2D/Collision/b2CollidePolygon.cpp`
- `src/Vixen.Common/Box2D/Collision/b2Collision.cpp`
- `src/Vixen.Common/Box2D/Collision/b2Collision.h`
- `src/Vixen.Common/Box2D/Collision/b2Distance.cpp`
- `src/Vixen.Common/Box2D/Collision/b2Distance.h`
- `src/Vixen.Common/Box2D/Collision/b2DynamicTree.cpp`
- `src/Vixen.Common/Box2D/Collision/b2DynamicTree.h`
- `src/Vixen.Common/Box2D/Collision/b2TimeOfImpact.cpp`
- `src/Vixen.Common/Box2D/Collision/b2TimeOfImpact.h`
- `src/Vixen.Common/Box2D/Common/b2BlockAllocator.cpp`
- `src/Vixen.Common/Box2D/Common/b2BlockAllocator.h`
- `src/Vixen.Common/Box2D/Common/b2Draw.h`
- `src/Vixen.Common/Box2D/Common/b2GrowableStack.h`
- `src/Vixen.Common/Box2D/Common/b2Math.cpp`
- `src/Vixen.Common/Box2D/Common/b2Math.h`
- `src/Vixen.Common/Box2D/Common/b2Settings.cpp`
- `src/Vixen.Common/Box2D/Common/b2Settings.h`
- `src/Vixen.Common/Box2D/Common/b2StackAllocator.cpp`
- `src/Vixen.Common/Box2D/Common/b2StackAllocator.h`
- `src/Vixen.Common/Box2D/Common/b2Stat.cpp`
- `src/Vixen.Common/Box2D/Common/b2Stat.h`
- `src/Vixen.Common/Box2D/Common/b2Timer.cpp`
- `src/Vixen.Common/Box2D/Common/b2Timer.h`
- `src/Vixen.Common/Box2D/Common/b2TrackedBlock.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ChainAndCircleContact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ChainAndPolygonContact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2CircleContact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2Contact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2Contact.h`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ContactSolver.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ContactSolver.h`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2EdgeAndCircleContact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2EdgeAndPolygonContact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2PolygonAndCircleContact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Contacts/b2PolygonContact.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2DistanceJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2DistanceJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2FrictionJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2FrictionJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2GearJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2GearJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2Joint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2Joint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2MotorJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2MotorJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2MouseJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2MouseJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2PrismaticJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2PrismaticJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2PulleyJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2PulleyJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2RevoluteJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2RevoluteJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2RopeJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2RopeJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2WeldJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2WeldJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2WheelJoint.cpp`
- `src/Vixen.Common/Box2D/Dynamics/Joints/b2WheelJoint.h`
- `src/Vixen.Common/Box2D/Dynamics/b2Body.cpp`
- `src/Vixen.Common/Box2D/Dynamics/b2Body.h`
- `src/Vixen.Common/Box2D/Dynamics/b2ContactManager.h`
- `src/Vixen.Common/Box2D/Dynamics/b2Fixture.h`
- `src/Vixen.Common/Box2D/Dynamics/b2Island.cpp`
- `src/Vixen.Common/Box2D/Dynamics/b2Island.h`
- `src/Vixen.Common/Box2D/Dynamics/b2TimeStep.h`
- `src/Vixen.Common/Box2D/Dynamics/b2World.cpp`
- `src/Vixen.Common/Box2D/Dynamics/b2World.h`
- `src/Vixen.Common/Box2D/Dynamics/b2WorldCallbacks.h`
- `src/Vixen.Common/Box2D/Particle/b2Particle.cpp`
- `src/Vixen.Common/Box2D/Particle/b2Particle.h`
- `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.cpp`
- `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.h`
- `src/Vixen.Common/Box2D/Particle/b2ParticleSystem.cpp`
- `src/Vixen.Common/Box2D/Particle/b2ParticleSystem.h`
- `src/Vixen.Common/Box2D/Rope/b2Rope.cpp`
- `src/Vixen.Common/Box2D/Rope/b2Rope.h`
- `src/Vixen.Modules/Analysis/QMLibrary/VampParamCtrl.h`
- `src/Vixen.Modules/Effect/Liquid/LiquidFunWrapper/LiquidFunWrapper.cpp`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/Box2D/Collision/Shapes/b2ChainShape.cpp` | GetChildCount, ~b2ChainShape, Clone, RayCast, TestPoint, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2ChainShape.h` | b2EdgeShape, b2ChainShape, b2ChainShape |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2CircleShape.cpp` | Clone, ComputeAABB, TestPoint, RayCast, ComputeMass, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2CircleShape.h` | b2CircleShape, b2CircleShape, GetSupport, GetSupportVertex, GetVertex, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2EdgeShape.cpp` | ComputeMass, TestPoint, Set, ComputeAABB, ComputeDistance, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2EdgeShape.h` | b2EdgeShape, b2EdgeShape, Set |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2PolygonShape.cpp` | ComputeAABB, Set, ComputeMass, GetChildCount, Validate, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2PolygonShape.h` | b2PolygonShape, b2PolygonShape, SetAsBox, SetCentroid, GetVertexCount, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2Shape.h` | b2Shape, ~b2Shape, GetType, b2MassData, Type |
| `src/Vixen.Common/Box2D/Collision/b2BroadPhase.cpp` | CreateProxy, BufferMove, DestroyProxy, UnBufferMove, QueryCallback, ... |
| `src/Vixen.Common/Box2D/Collision/b2BroadPhase.h` | UpdatePairs, b2Pair, b2PairLessThan, GetTreeQuality, ShiftOrigin, ... |
| `src/Vixen.Common/Box2D/Collision/b2CollideCircle.cpp` | b2CollideCircles, b2CollidePolygonAndCircle |
| `src/Vixen.Common/Box2D/Collision/b2CollideEdge.cpp` | ComputeEdgeSeparation, b2TempPolygon, b2CollideEdgeAndCircle, VertexType, b2ReferenceFace, ... |
| `src/Vixen.Common/Box2D/Collision/b2CollidePolygon.cpp` | b2FindMaxSeparation, b2CollidePolygons, b2FindIncidentEdge |
| `src/Vixen.Common/Box2D/Collision/b2Collision.cpp` | b2GetPointStates, b2ClipSegmentToLine, b2TestOverlap, RayCast, Initialize |
| `src/Vixen.Common/Box2D/Collision/b2Collision.h` | b2PointState, b2Manifold, b2ManifoldPoint, b2PolygonShape, IsValid, ... |
| `src/Vixen.Common/Box2D/Collision/b2Distance.cpp` | b2Simplex, Set, b2Distance, b2SimplexVertex, Solve3, ... |
| `src/Vixen.Common/Box2D/Collision/b2Distance.h` | GetVertex, GetSupport, b2DistanceOutput, GetSupportVertex, b2SimplexCache, ... |
| `src/Vixen.Common/Box2D/Collision/b2DynamicTree.cpp` | MoveProxy, GetMaxBalance, b2DynamicTree, FreeNode, ValidateStructure, ... |
| `src/Vixen.Common/Box2D/Collision/b2DynamicTree.h` | b2DynamicTree, Query, RayCast, b2TreeNode, GetFatAABB, ... |
| `src/Vixen.Common/Box2D/Collision/b2TimeOfImpact.cpp` | b2SeparationFunction, b2TimeOfImpact |
| `src/Vixen.Common/Box2D/Collision/b2TimeOfImpact.h` | b2TOIInput, b2TOIOutput |
| `src/Vixen.Common/Box2D/Common/b2BlockAllocator.cpp` | Allocate, Clear, ~b2BlockAllocator, b2Block, b2BlockAllocator, ... |
| `src/Vixen.Common/Box2D/Common/b2BlockAllocator.h` | b2BlockAllocator |
| `src/Vixen.Common/Box2D/Common/b2Draw.h` | b2Color, b2Color |
| `src/Vixen.Common/Box2D/Common/b2GrowableStack.h` | Pop, b2GrowableStack, b2GrowableStack, GetCount, ~b2GrowableStack, ... |
| `src/Vixen.Common/Box2D/Common/b2Math.cpp` | GetSymInverse33, Solve22, GetInverse22, Solve33 |
| `src/Vixen.Common/Box2D/Common/b2Math.h` | b2Cross, b2MulT, b2Mul, b2MulT, b2Abs, ... |
| `src/Vixen.Common/Box2D/Common/b2Settings.cpp` | b2Alloc, b2GetNumAllocs, b2SetAllocFreeCallbacks, b2AllocDefault, b2FreeDefault, ... |
| `src/Vixen.Common/Box2D/Common/b2Settings.h` | int64, int32, float64, float32, uint16 |
| `src/Vixen.Common/Box2D/Common/b2StackAllocator.cpp` | Reallocate, Free, Allocate, GetMaxAllocation |
| `src/Vixen.Common/Box2D/Common/b2StackAllocator.h` | b2StackEntry, b2StackAllocator |
| `src/Vixen.Common/Box2D/Common/b2Stat.cpp` | GetMax, GetMin, Record, GetMean |
| `src/Vixen.Common/Box2D/Common/b2Stat.h` | b2Stat |
| `src/Vixen.Common/Box2D/Common/b2Timer.cpp` | GetTicks, GetTicks, GetMilliseconds, b2Timer, GetMilliseconds, ... |
| `src/Vixen.Common/Box2D/Common/b2Timer.h` | b2Timer |
| `src/Vixen.Common/Box2D/Common/b2TrackedBlock.cpp` | Free |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ChainAndCircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ChainAndPolygonContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2CircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2Contact.cpp` | InitializeRegisters, Create, AddType |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2Contact.h` | b2ContactEdge, b2StackAllocator, GetWorldManifold, b2Body |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ContactSolver.cpp` | b2PositionSolverManifold, SolveVelocityConstraints, b2ContactSolver, SolveTOIPositionConstraints, b2ContactPositionConstraint, ... |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ContactSolver.h` | b2ContactSolver, b2ContactVelocityConstraint, b2ContactSolverDef, b2VelocityConstraintPoint |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2EdgeAndCircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2EdgeAndPolygonContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2PolygonAndCircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2PolygonContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2DistanceJoint.cpp` | b2DistanceJoint, Initialize, SolveVelocityConstraints, GetReactionTorque, GetReactionForce, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2DistanceJoint.h` | GetFrequency, GetLocalAnchorA, SetFrequency, GetLocalAnchorB, GetLength, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2FrictionJoint.cpp` | GetMaxTorque, GetReactionForce, SolveVelocityConstraints, SetMaxTorque, SolvePositionConstraints, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2FrictionJoint.h` | GetLocalAnchorA, GetLocalAnchorB, b2FrictionJointDef, b2FrictionJoint |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2GearJoint.cpp` | GetAnchorA, GetReactionForce, GetRatio, SolvePositionConstraints, InitVelocityConstraints, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2GearJoint.h` | b2GearJoint, GetJoint2, b2GearJointDef, GetJoint1 |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2Joint.cpp` | b2Joint |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2Joint.h` | b2JointDef, GetNext, GetBodyB, SetUserData, GetNext, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MotorJoint.cpp` | InitVelocityConstraints, SetMaxForce, SolvePositionConstraints, GetLinearOffset, GetMaxTorque, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MotorJoint.h` | b2MotorJointDef, b2MotorJoint |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MouseJoint.cpp` | GetTarget, SolvePositionConstraints, SetMaxForce, GetAnchorA, GetReactionTorque, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MouseJoint.h` | b2MouseJointDef, b2MouseJoint |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PrismaticJoint.cpp` | GetReactionForce, b2PrismaticJoint, GetAnchorA, InitVelocityConstraints, GetAnchorB, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PrismaticJoint.h` | b2PrismaticJointDef, GetLocalAxisA, GetLocalAnchorB, GetReferenceAngle, b2PrismaticJoint, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PulleyJoint.cpp` | GetLengthA, InitVelocityConstraints, GetRatio, Initialize, GetGroundAnchorA, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PulleyJoint.h` | b2PulleyJoint, b2PulleyJointDef |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RevoluteJoint.cpp` | GetJointSpeed, GetLowerLimit, GetReactionForce, GetReactionTorque, SolvePositionConstraints, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RevoluteJoint.h` | GetMotorSpeed, GetLocalAnchorA, GetLocalAnchorB, b2RevoluteJointDef, GetMaxMotorTorque, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RopeJoint.cpp` | InitVelocityConstraints, GetAnchorB, GetMaxLength, SolvePositionConstraints, GetLimitState, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RopeJoint.h` | GetLocalAnchorB, b2RopeJoint, SetMaxLength, GetLocalAnchorA, b2RopeJointDef |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WeldJoint.cpp` | Initialize, GetReactionForce, SolvePositionConstraints, GetAnchorB, GetAnchorA, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WeldJoint.h` | GetLocalAnchorA, b2WeldJoint, GetReferenceAngle, b2WeldJointDef, GetDampingRatio, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WheelJoint.cpp` | GetAnchorA, GetMotorTorque, b2WheelJoint, GetReactionForce, SolveVelocityConstraints, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WheelJoint.h` | GetLocalAnchorB, b2WheelJointDef, GetMotorSpeed, GetSpringFrequencyHz, GetSpringDampingRatio, ... |
| `src/Vixen.Common/Box2D/Dynamics/b2Body.cpp` | b2Body |
| `src/Vixen.Common/Box2D/Dynamics/b2Body.h` | GetWorldPoint, GetLocalVector, GetLinearVelocityFromWorldPoint, b2BodyDef, GetTransform, ... |
| `src/Vixen.Common/Box2D/Dynamics/b2ContactManager.h` | b2ParticleSystem |
| `src/Vixen.Common/Box2D/Dynamics/b2Fixture.h` | GetBody, TestPoint, SetFriction, SetRestitution, GetBody, ... |
| `src/Vixen.Common/Box2D/Dynamics/b2Island.cpp` | SolveTOI, Report, Solve |
| `src/Vixen.Common/Box2D/Dynamics/b2Island.h` | b2ContactVelocityConstraint |
| `src/Vixen.Common/Box2D/Dynamics/b2TimeStep.h` | b2Velocity, b2Position, b2SolverData |
| `src/Vixen.Common/Box2D/Dynamics/b2World.cpp` | DrawShape, QueryAABB, GetTreeQuality, DrawJoint, GetTreeBalance, ... |
| `src/Vixen.Common/Box2D/Dynamics/b2World.h` | b2World, SetGravity, b2AABB, b2Color, SetGravity |
| `src/Vixen.Common/Box2D/Dynamics/b2WorldCallbacks.h` | ReportParticle, b2RayCastCallback, EndContact, ~b2RayCastCallback, b2ContactImpulse, ... |
| `src/Vixen.Common/Box2D/Particle/b2Particle.cpp` | Set, b2CalculateParticleIterations, GetColor, b2ParticleColor |
| `src/Vixen.Common/Box2D/Particle/b2Particle.h` | b2Color, b2ParticleFlag |
| `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.cpp` | FreeShapesMemory, SetCircleShapesFromVertexList |
| `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.h` | b2CircleShape |
| `src/Vixen.Common/Box2D/Particle/b2ParticleSystem.cpp` | NewIndices, LimitCapacity, operator(), SetColorBuffer, RayCast, ... |
| `src/Vixen.Common/Box2D/Particle/b2ParticleSystem.h` | UserOverridableBuffer, GetRadius |
| `src/Vixen.Common/Box2D/Rope/b2Rope.cpp` | SetAngle, Initialize, ~b2Rope, SolveC3, Step, ... |
| `src/Vixen.Common/Box2D/Rope/b2Rope.h` | b2Draw, b2RopeDef, GetVertexCount, GetVertices, b2Rope |
| `src/Vixen.Modules/Analysis/QMLibrary/VampParamCtrl.h` | each |
| `src/Vixen.Modules/Effect/Liquid/LiquidFunWrapper/LiquidFunWrapper.cpp` | CreateParticles, ConvertParticleType, CreateBarrier, StepWorld, Initialize, ... |

## Connected Communities

- **Vixen.Common/Box2D · SetAwake** (12 cross-edges)
- **Box2D/Particle +3 dirs** (6 cross-edges)
- **Vixen.Common/Box2D · Free** (2 cross-edges)
- **Box2D/Common · FreeEmptySlabs** (1 cross-edges)
- **Box2D/Particle +2 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-39")
explore(operation:"context", task:"understand Dynamics/Joints +9 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
