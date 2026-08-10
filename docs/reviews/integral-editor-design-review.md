# Integral editor design review

## Context

`IntegerEditor` is a WPF control whose dependency properties and calculation fields are currently
typed as `int`. The Effect Editor also needs the same interaction model for `byte`, `ushort`, `uint`,
and `ulong` properties without maintaining a control and resource dictionary for every numeric type.

## Options considered

### Generic WPF control

An `IntegerEditor<T>` could share its C# implementation through generic math. However, WPF resource
dictionaries cannot instantiate or target an open generic type. Each supported closed type would
still need a concrete XAML-visible control type, style registration, and editor resource entry. This
moves duplication instead of eliminating it and makes editor discovery more complex.

### One runtime-typed integral control

Keep one `IntegerEditor` control and one XAML template. Its `Value` dependency property can accept a
boxed supported integral value, remember that runtime type, perform calculations using `decimal`,
and convert the result back to the original integral type before the two-way binding updates the
property model. `decimal` is appropriate for the internal calculation because it represents the
entire `ulong` range exactly.

## Recommendation

Use the runtime-typed control. Register each supported property type in `EditorCollection` with its
actual edited type and the existing integer editor key. Intersect configured `NumberRange` limits
with the intrinsic limits of the runtime value type, preventing overflow even when no range metadata
is present. Retain `IntegerEditor` as the public control name to avoid unnecessary XAML and API churn.

This design uses the Strategy pattern at the conversion boundary: the runtime integral type selects
the appropriate bounds and result conversion while the editor interaction remains shared. It keeps
the control cohesive, preserves exact values, and requires no factory or dependency-injection changes.
