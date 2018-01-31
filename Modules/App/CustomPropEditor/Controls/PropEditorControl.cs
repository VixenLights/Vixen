using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VixenModules.App.CustomPropEditor.Controls
{
    public class PropEditorControl : Control
    {

        static PropEditorControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(PropEditorControl),
                new FrameworkPropertyMetadata(typeof(PropEditorControl)));
        }


        #region Properties

        #region CurrentGeometry Property

        internal GeometryGroup SelectedGeometries
        {
            get { return (GeometryGroup)GetValue(SelectedGeometriesProperty); }
            set { SetValue(SelectedGeometriesProperty, value); }
        }

        /// <value>Identifies the CurrentGeometry dependency property</value>
        internal static readonly DependencyProperty SelectedGeometriesProperty =
            DependencyProperty.Register(
                "SelectedGeometries",
                typeof(GeometryGroup),
                typeof(PropEditorControl),
                new FrameworkPropertyMetadata(default(GeometryGroup), CurrentGeometryChanged));

        /// <summary>
        /// Invoked on CurrentGeometry change.
        /// </summary>
        /// <param name="d">The object that was changed</param>
        /// <param name="e">Dependency property changed event arguments</param>
        private static void CurrentGeometryChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region IsDrawingEnabled Property

        public bool IsDrawingEnabled
        {
            get { return (bool)GetValue(IsDrawingEnabledProperty); }
            set { SetValue(IsDrawingEnabledProperty, value); }
        }

        /// <value>Identifies the IsDrawingEnabled dependency property</value>
        public static readonly DependencyProperty IsDrawingEnabledProperty =
            DependencyProperty.Register(
                "IsDrawingEnabled",
                typeof(bool),
                typeof(PropEditorControl),
                new FrameworkPropertyMetadata(default(bool), IsDrawingEnabledChanged));

        /// <summary>
        /// Invoked on IsDrawingEnabled change.
        /// </summary>
        /// <param name="d">The object that was changed</param>
        /// <param name="e">Dependency property changed event arguments</param>
        private static void IsDrawingEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Should end drawing here if already begun.
        }

        #endregion

        #region GeometriesSource Property

        public ICollection<Geometry> GeometriesSource
        {
            get { return (ICollection<Geometry>)GetValue(GeometriesSourceProperty); }
            set { SetValue(GeometriesSourceProperty, value); }
        }

        /// <value>Identifies the GeometriesSource dependency property</value>
        public static readonly DependencyProperty GeometriesSourceProperty =
            DependencyProperty.Register(
                "GeometriesSource",
                typeof(ICollection<Geometry>),
                typeof(PropEditorControl),
                new FrameworkPropertyMetadata(default(ICollection<Geometry>), GeometriesSourceChanged));

        /// <summary>
        /// Invoked on GeometriesSource change.
        /// </summary>
        /// <param name="d">The object that was changed</param>
        /// <param name="e">Dependency property changed event arguments</param>
        private static void GeometriesSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #region GeometryTemplate Property

        public DataTemplate GeometryTemplate
        {
            get { return (DataTemplate)GetValue(GeometryTemplateProperty); }
            set { SetValue(GeometryTemplateProperty, value); }
        }

        /// <value>Identifies the GeometryTemplate dependency property</value>
        public static readonly DependencyProperty GeometryTemplateProperty =
            DependencyProperty.Register(
                "GeometryTemplate",
                typeof(DataTemplate),
                typeof(PropEditorControl),
                new FrameworkPropertyMetadata(default(DataTemplate), GeometryTemplateChanged));

        /// <summary>
        /// Invoked on GeometryTemplate change.
        /// </summary>
        /// <param name="d">The object that was changed</param>
        /// <param name="e">Dependency property changed event arguments</param>
        private static void GeometryTemplateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
        }

        #endregion

        #endregion

    }
}
