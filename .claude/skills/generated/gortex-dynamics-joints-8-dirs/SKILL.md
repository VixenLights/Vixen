---
name: gortex-dynamics-joints-8-dirs
description: "Work in the Dynamics/Joints +8 dirs area — 522 symbols across 85 files (87% cohesion)"
---

# Dynamics/Joints +8 dirs

522 symbols | 85 files | 87% cohesion

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
- `src/Vixen.Common/Box2D/Dynamics/b2Fixture.h`
- `src/Vixen.Common/Box2D/Dynamics/b2Island.cpp`
- `src/Vixen.Common/Box2D/Dynamics/b2Island.h`
- `src/Vixen.Common/Box2D/Dynamics/b2TimeStep.h`
- `src/Vixen.Common/Box2D/Dynamics/b2World.cpp`
- `src/Vixen.Common/Box2D/Dynamics/b2WorldCallbacks.h`
- `src/Vixen.Common/Box2D/Particle/b2Particle.cpp`
- `src/Vixen.Common/Box2D/Particle/b2Particle.h`
- `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.cpp`
- `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.h`
- `src/Vixen.Common/Box2D/Particle/b2ParticleSystem.cpp`
- `src/Vixen.Common/Box2D/Rope/b2Rope.cpp`
- `src/Vixen.Common/Box2D/Rope/b2Rope.h`
- `src/Vixen.Modules/Effect/Liquid/LiquidFunWrapper/LiquidFunWrapper.cpp`

## Key Files

| File | Symbols |
|------|---------|
| `src/Vixen.Common/Box2D/Collision/Shapes/b2ChainShape.cpp` | CreateLoop, SetPrevVertex, SetNextVertex, CreateChain, ComputeAABB, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2ChainShape.h` | b2ChainShape, b2EdgeShape, b2ChainShape |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2CircleShape.cpp` | RayCast, ComputeAABB, TestPoint, ComputeDistance, ComputeMass, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2CircleShape.h` | GetSupport, GetVertex, GetVertexCount, b2CircleShape, GetSupportVertex, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2EdgeShape.cpp` | RayCast, ComputeDistance, ComputeMass, ComputeAABB, Clone, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2EdgeShape.h` | Set, b2EdgeShape, b2EdgeShape |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2PolygonShape.cpp` | ComputeDistance, Validate, SetAsBox, Set, ComputeCentroid, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2PolygonShape.h` | SetAsBox, b2PolygonShape, b2PolygonShape, GetVertex, GetVertexCount, ... |
| `src/Vixen.Common/Box2D/Collision/Shapes/b2Shape.h` | Type, ~b2Shape, b2MassData, GetType, b2Shape |
| `src/Vixen.Common/Box2D/Collision/b2BroadPhase.cpp` | ~b2BroadPhase, b2BroadPhase, MoveProxy, BufferMove, CreateProxy, ... |
| `src/Vixen.Common/Box2D/Collision/b2BroadPhase.h` | GetTreeQuality, GetTreeHeight |
| `src/Vixen.Common/Box2D/Collision/b2CollideCircle.cpp` | b2CollidePolygonAndCircle, b2CollideCircles |
| `src/Vixen.Common/Box2D/Collision/b2CollideEdge.cpp` | b2EPCollider, b2CollideEdgeAndCircle, b2ReferenceFace, b2TempPolygon, Collide, ... |
| `src/Vixen.Common/Box2D/Collision/b2CollidePolygon.cpp` | b2CollidePolygons, b2FindIncidentEdge, b2FindMaxSeparation |
| `src/Vixen.Common/Box2D/Collision/b2Collision.cpp` | RayCast, b2TestOverlap, b2GetPointStates, Initialize, b2ClipSegmentToLine |
| `src/Vixen.Common/Box2D/Collision/b2Collision.h` | b2PolygonShape, b2RayCastInput, b2Shape, b2AABB, b2PointState, ... |
| `src/Vixen.Common/Box2D/Collision/b2Distance.cpp` | Solve2, Solve3, b2SimplexVertex, Set, b2Distance, ... |
| `src/Vixen.Common/Box2D/Collision/b2Distance.h` | GetSupportVertex, b2DistanceOutput, GetVertexCount, GetSupport, b2SimplexCache, ... |
| `src/Vixen.Common/Box2D/Collision/b2DynamicTree.cpp` | InsertLeaf, GetAreaRatio, ValidateMetrics, ComputeHeight, ValidateStructure, ... |
| `src/Vixen.Common/Box2D/Collision/b2DynamicTree.h` | b2TreeNode, GetUserData, Query, b2DynamicTree, RayCast, ... |
| `src/Vixen.Common/Box2D/Collision/b2TimeOfImpact.cpp` | b2TimeOfImpact, b2SeparationFunction |
| `src/Vixen.Common/Box2D/Collision/b2TimeOfImpact.h` | b2TOIOutput, b2TOIInput |
| `src/Vixen.Common/Box2D/Common/b2BlockAllocator.cpp` | ~b2BlockAllocator, Allocate, b2Chunk, b2Block, Clear, ... |
| `src/Vixen.Common/Box2D/Common/b2BlockAllocator.h` | b2BlockAllocator |
| `src/Vixen.Common/Box2D/Common/b2GrowableStack.h` | b2GrowableStack, Push, b2GrowableStack, GetCount, ~b2GrowableStack, ... |
| `src/Vixen.Common/Box2D/Common/b2Math.cpp` | GetSymInverse33, Solve22, GetInverse22, Solve33 |
| `src/Vixen.Common/Box2D/Common/b2Math.h` | b2MulT, b2Mul, b2Cross, GetTransform, b2Abs, ... |
| `src/Vixen.Common/Box2D/Common/b2Settings.cpp` | b2Free, b2Alloc |
| `src/Vixen.Common/Box2D/Common/b2Settings.h` | float32, int64, uint16, float64 |
| `src/Vixen.Common/Box2D/Common/b2StackAllocator.cpp` | Reallocate, Allocate, Free |
| `src/Vixen.Common/Box2D/Common/b2StackAllocator.h` | b2StackAllocator, b2StackEntry |
| `src/Vixen.Common/Box2D/Common/b2Stat.cpp` | GetMean, GetMax, GetMin, Record |
| `src/Vixen.Common/Box2D/Common/b2Stat.h` | b2Stat |
| `src/Vixen.Common/Box2D/Common/b2Timer.cpp` | Reset, GetTicks, Reset, GetTicks, b2Timer, ... |
| `src/Vixen.Common/Box2D/Common/b2Timer.h` | b2Timer |
| `src/Vixen.Common/Box2D/Common/b2TrackedBlock.cpp` | Free |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ChainAndCircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ChainAndPolygonContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2CircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2Contact.h` | GetWorldManifold, b2ContactEdge, b2StackAllocator, b2Body |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ContactSolver.cpp` | WarmStart, InitializeVelocityConstraints, SolveVelocityConstraints, SolvePositionConstraints, StoreImpulses, ... |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2ContactSolver.h` | b2ContactSolverDef, b2ContactSolver, b2ContactVelocityConstraint, b2VelocityConstraintPoint |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2EdgeAndCircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2EdgeAndPolygonContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2PolygonAndCircleContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Contacts/b2PolygonContact.cpp` | Evaluate |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2DistanceJoint.cpp` | GetAnchorB, b2DistanceJoint, GetReactionTorque, SolvePositionConstraints, GetReactionForce, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2DistanceJoint.h` | GetLocalAnchorA, GetDampingRatio, GetFrequency, b2DistanceJoint, GetLength, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2FrictionJoint.cpp` | GetAnchorA, InitVelocityConstraints, GetReactionForce, b2FrictionJoint, SetMaxForce, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2FrictionJoint.h` | GetLocalAnchorB, b2FrictionJoint, GetLocalAnchorA, b2FrictionJointDef |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2GearJoint.cpp` | SolvePositionConstraints, GetAnchorA, GetAnchorB, b2GearJoint, SolveVelocityConstraints, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2GearJoint.h` | b2GearJointDef |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2Joint.h` | GetBodyA, b2Jacobian, b2SolverData, b2LimitState, GetBodyB, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MotorJoint.cpp` | SolvePositionConstraints, GetReactionForce, InitVelocityConstraints, GetMaxTorque, GetAnchorA, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MotorJoint.h` | b2MotorJoint, b2MotorJointDef |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MouseJoint.cpp` | GetReactionForce, GetAnchorB, b2MouseJoint, InitVelocityConstraints, GetAnchorA, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2MouseJoint.h` | b2MouseJoint, b2MouseJointDef, Dump |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PrismaticJoint.cpp` | GetUpperLimit, GetJointTranslation, b2PrismaticJoint, SolvePositionConstraints, GetAnchorA, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PrismaticJoint.h` | GetMotorSpeed, GetReferenceAngle, GetLocalAnchorB, GetMaxMotorForce, GetLocalAnchorA, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PulleyJoint.cpp` | GetAnchorB, GetGroundAnchorA, GetCurrentLengthB, ShiftOrigin, GetRatio, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2PulleyJoint.h` | b2PulleyJoint, b2PulleyJointDef |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RevoluteJoint.cpp` | GetReactionTorque, b2RevoluteJoint, GetMotorTorque, SolveVelocityConstraints, GetAnchorB, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RevoluteJoint.h` | GetLocalAnchorB, GetMaxMotorTorque, GetReferenceAngle, b2RevoluteJoint, b2RevoluteJointDef, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RopeJoint.cpp` | InitVelocityConstraints, GetAnchorB, GetLimitState, SolvePositionConstraints, b2RopeJoint, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2RopeJoint.h` | GetLocalAnchorB, b2RopeJointDef, SetMaxLength, GetLocalAnchorA, b2RopeJoint |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WeldJoint.cpp` | GetReactionTorque, SolveVelocityConstraints, GetAnchorB, GetReactionForce, SolvePositionConstraints, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WeldJoint.h` | SetFrequency, b2WeldJointDef, SetDampingRatio, GetLocalAnchorA, GetDampingRatio, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WheelJoint.cpp` | GetReactionTorque, GetMotorTorque, GetJointTranslation, GetAnchorA, GetReactionForce, ... |
| `src/Vixen.Common/Box2D/Dynamics/Joints/b2WheelJoint.h` | SetSpringDampingRatio, GetLocalAnchorB, GetLocalAnchorA, GetSpringDampingRatio, GetMaxMotorTorque, ... |
| `src/Vixen.Common/Box2D/Dynamics/b2Body.cpp` | b2Body |
| `src/Vixen.Common/Box2D/Dynamics/b2Body.h` | SetTransform, GetTransform, SetPosition, b2BodyDef, GetWorldVector, ... |
| `src/Vixen.Common/Box2D/Dynamics/b2Fixture.h` | SetFriction, GetShape, GetShape, SetRestitution, GetNext, ... |
| `src/Vixen.Common/Box2D/Dynamics/b2Island.cpp` | Report, SolveTOI, Solve |
| `src/Vixen.Common/Box2D/Dynamics/b2Island.h` | b2ContactVelocityConstraint |
| `src/Vixen.Common/Box2D/Dynamics/b2TimeStep.h` | b2Velocity, b2Position, b2SolverData |
| `src/Vixen.Common/Box2D/Dynamics/b2World.cpp` | DrawJoint, GetTreeQuality |
| `src/Vixen.Common/Box2D/Dynamics/b2WorldCallbacks.h` | PostSolve, b2ContactImpulse |
| `src/Vixen.Common/Box2D/Particle/b2Particle.cpp` | Set, b2ParticleColor |
| `src/Vixen.Common/Box2D/Particle/b2Particle.h` | b2Color |
| `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.cpp` | FreeShapesMemory, SetCircleShapesFromVertexList |
| `src/Vixen.Common/Box2D/Particle/b2ParticleGroup.h` | b2CircleShape |
| `src/Vixen.Common/Box2D/Particle/b2ParticleSystem.cpp` | RayCast |
| `src/Vixen.Common/Box2D/Rope/b2Rope.cpp` | Step, SolveC2, SetAngle, ~b2Rope, Initialize, ... |
| `src/Vixen.Common/Box2D/Rope/b2Rope.h` | b2Rope, GetVertices, GetVertexCount, b2RopeDef |
| `src/Vixen.Modules/Effect/Liquid/LiquidFunWrapper/LiquidFunWrapper.cpp` | CreateBarrier, Initialize |

## Connected Communities

- **Vixen.Common/Box2D · SetAwake** (8 cross-edges)
- **Box2D/Particle +5 dirs** (2 cross-edges)
- **Vixen.Common/Box2D · b2Log** (1 cross-edges)
- **Box2D/Particle +2 dirs** (1 cross-edges)

## How to Explore

```
analyze(operation:"communities", id:"community-41")
explore(operation:"context", task:"understand Dynamics/Joints +8 dirs", format:"gcx")
```

_`format: "gcx"` returns the [GCX1 compact wire format](../../docs/wire-format.md) — round-trippable, ~27% fewer tokens than JSON. Drop it for JSON output; agents using `@gortex/wire` or the Go `github.com/gortexhq/gcx-go` package decode either._
