---
name: catel-mvvm
description: |
  Ensure .NET/C# WPF code meets best practices for the solution/project.
---

# .NET/C# WPF With Catel MVVM Best Practices

C# WPF coding guidelines and best practices. MUST follow these rules. Use when reviewing/writing C# WPF code.

##  ViewModel Definition and Registration

- Base Class: Always inherit from Catel.MVVM.ViewModelBase.
- Services: Inject services (e.g., IUIVisualizerService, IMessageService, INavigationService) via the constructor. Do not use global state.
- Constructor Injection: ViewModels should not use the service locator pattern. Enforce constructor injection for all services.
- Initialization: Place time-consuming asynchronous initialization logic inside the InitializeAsync() override, rather than inside the constructor.
- Saving/Canceling: Override SaveAsync() and CancelAsync() to manage data persistence and dialog results correctly.

## Properties Model vs ViewModel

- Dependency Properties: Wrap bound properties using Catel.MVVM.ViewModelToModelAttribute when mirroring a domain model.
- Field-based Properties: Use Catel's RegisterProperty<T> or AdvancedPropertyChanged instead of standard INotifyPropertyChanged boilerplate.
- Validation: Implement ValidateFields and ValidateBusinessRules overrides to encapsulate data integrity rules at the ViewModel level rather than pushing logic to the code-behind.

## Commands & Interactivity

- Command Binding: Use Catel.MVVM.Command or Catel.MVVM.TaskCommand for asynchronous operations.
- CanExecute: Always define corresponding Can[Action] methods (returning bool) for every [Action]Command to automatically handle UI button states.
- Code-Behind Restriction: Do not write UI event handlers in .xaml.cs files unless strictly necessary.
  Use Catel's command bindings or attached behaviors for WPF UI interactions.

## Views and Data Binding

- Base Class: Ensure views inherit from Catel.Windows.Controls.Window or Catel.Windows.Controls.UserControl.
- View-to-ViewModel Mapping: Rely on Catel's automatic view model locator rather than explicitly setting DataContext in code.
- Compiled Bindings: Wherever possible in Catel Views, use {Binding RelativeSource={RelativeSource FindAncestor, AncestorType={x:Type catel:UserControl}}, Path=ViewModel.PropertyName} to ensure binding safety.

## Messaging and Orchestration

- Messenger: Use Catel.Messaging.IMessageMediator for cross-ViewModel communication instead of tight coupling.
- Life-cycle Registration: Ensure message subscriptions are safely registered and deregistered to prevent memory leaks in WPF.