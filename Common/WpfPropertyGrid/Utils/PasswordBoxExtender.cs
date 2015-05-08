/*
 * Copyright © 2010, Denys Vuika
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *  http://www.apache.org/licenses/LICENSE-2.0
 *  
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 */
namespace System.Windows.Controls.WpfPropertyGrid
{
  /// <summary>
  /// Provides binding support for PasswordBox.  
  /// </summary>
  public class PasswordBoxExtender
  {
    /// <summary>
    /// Identifies the <see cref="BoundPasswordProperty"/> attached property.
    /// </summary>
    public static readonly DependencyProperty BoundPasswordProperty =
      DependencyProperty.RegisterAttached("BoundPassword", typeof(string), typeof(PasswordBoxExtender),
      new FrameworkPropertyMetadata(string.Empty, OnBoundPasswordChanged));

    /// <summary>
    /// Identifies the <see cref="BindPasswordProperty"/> attached property.
    /// </summary>
    public static readonly DependencyProperty BindPasswordProperty =
      DependencyProperty.RegisterAttached("BindPassword", typeof(bool), typeof(PasswordBoxExtender),
      new PropertyMetadata(false, OnBindPasswordChanged));

    private static readonly DependencyProperty UpdatingPasswordProperty =
      DependencyProperty.RegisterAttached("UpdatingPassword", typeof(bool), typeof(PasswordBoxExtender));

    private static void OnBoundPasswordChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
    {
      var box = d as PasswordBox;

      // only handle this event when the property is attached to a PasswordBox   
      // and when the BindPasswordProperty attached property has been set to true   
      if (d == null || !GetBindPassword(d)) return;

      // avoid recursive updating by ignoring the box's changed event   
      box.PasswordChanged -= HandlePasswordChanged;

      string newPassword = (string)e.NewValue;

      if (!GetUpdatingPassword(box))
        box.Password = newPassword;

      box.PasswordChanged += HandlePasswordChanged;
    }

    private static void OnBindPasswordChanged(DependencyObject dp, DependencyPropertyChangedEventArgs e)
    {
      // when the BindPasswordProperty attached property is set on a PasswordBox,   
      // start listening to its PasswordChanged event   

      var box = dp as PasswordBox;
      if (box == null) return;

      var wasBound = (bool)(e.OldValue);
      var needToBind = (bool)(e.NewValue);

      if (wasBound)
        box.PasswordChanged -= HandlePasswordChanged;

      if (needToBind)
        box.PasswordChanged += HandlePasswordChanged;
    }

    private static void HandlePasswordChanged(object sender, RoutedEventArgs e)
    {
      var box = sender as PasswordBox;

      // set a flag to indicate that we're updating the password   
      SetUpdatingPassword(box, true);
      // push the new password into the BoundPasswordProperty property   
      SetBoundPassword(box, box.Password);
      SetUpdatingPassword(box, false);
    }

    /// <summary>
    /// Sets the password bound.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <param name="value">if set to <c>true</c> [value].</param>
    public static void SetBindPassword(DependencyObject dp, bool value)
    {
      dp.SetValue(BindPasswordProperty, value);
    }

    /// <summary>
    /// Gets the password.
    /// </summary>
    /// <param name="dp">The dp.</param>
    /// <returns><c>true</c> if password was bound; otherwise, <c>false</c>.</returns>
    public static bool GetBindPassword(DependencyObject dp)
    {
      return (bool)dp.GetValue(BindPasswordProperty);
    }

    /// <summary>
    /// Gets the bound password.
    /// </summary>
    /// <param name="obj">The dependency object containing attached property value.</param>
    /// <returns>Password value.</returns>
    public static string GetBoundPassword(DependencyObject obj)
    {
      return (string)obj.GetValue(BoundPasswordProperty);
    }

    /// <summary>
    /// Sets the bound password.
    /// </summary>
    /// <param name="obj">The dependency object to contain attached property value.</param>
    /// <param name="value">The value.</param>
    public static void SetBoundPassword(DependencyObject obj, string value)
    {
      obj.SetValue(BoundPasswordProperty, value);
    }

    /// <summary>
    /// Gets the value determining whether the password updating is in progress.
    /// </summary>
    /// <param name="obj">The dependency object containing the value of attached property.</param>
    /// <returns></returns>
    private static bool GetUpdatingPassword(DependencyObject obj)
    {
      return (bool)obj.GetValue(UpdatingPasswordProperty);
    }
    
    /// <summary>
    /// Sets the value indicating the password updating is in progress.
    /// </summary>
    /// <param name="obj">The dependency object to assign attached property value.</param>
    /// <param name="value">if set to <c>true</c> the password is being updated.</param>
    private static void SetUpdatingPassword(DependencyObject obj, bool value)
    {
      obj.SetValue(UpdatingPasswordProperty, value);
    }
  }
}
