using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Animation;
using Othello.Utils;

namespace Othello.UniformBoard
{
    public class UniformBoard: Panel
    {

        #region Attached Properties

        #region Column

        public static int GetColumn(DependencyObject obj)
        {
            return (int)obj.GetValue(ColumnProperty);
        }

        public static void SetColumn(DependencyObject obj, int value)
        {
            obj.SetValue(ColumnProperty, value);
        }

        // Using a DependencyProperty as the backing store for Column.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnProperty =
            DependencyProperty.RegisterAttached("Column", typeof(int), typeof(UniformBoard), new UIPropertyMetadata(0, OnAttachedPropertyChanged));


        #endregion

        #region Row



        public static int GetRow(DependencyObject obj)
        {
            return (int)obj.GetValue(RowProperty);
        }

        public static void SetRow(DependencyObject obj, int value)
        {
            obj.SetValue(RowProperty, value);
        }

        // Using a DependencyProperty as the backing store for Row.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowProperty =
            DependencyProperty.RegisterAttached("Row", typeof(int), typeof(UniformBoard), new UIPropertyMetadata(0, OnAttachedPropertyChanged));



        #endregion

        #region RowSpan


        public static int GetRowSpan(DependencyObject obj)
        {
            return (int)obj.GetValue(RowSpanProperty);
        }

        public static void SetRowSpan(DependencyObject obj, int value)
        {
            obj.SetValue(RowSpanProperty, value);
        }

        // Using a DependencyProperty as the backing store for RowSpan.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowSpanProperty =
            DependencyProperty.RegisterAttached("RowSpan", typeof(int), typeof(UniformBoard), new UIPropertyMetadata(1));


        #endregion

        #endregion

        #region Dependency Properties

        #region LinesThickness

        public double LinesThickness
        {
            get { return (double)GetValue(SeparatorThicknessProperty); }
            set { SetValue(SeparatorThicknessProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LinesThickness.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeparatorThicknessProperty =
            DependencyProperty.Register("LinesThickness", typeof(double), typeof(UniformBoard),
                                        new UIPropertyMetadata(1d, OnDependencyPropertyChanged));

        #endregion

        #region ShowLines


        public bool ShowLines
        {
            get { return (bool)GetValue(ShowLinesProperty); }
            set { SetValue(ShowLinesProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ShowLines.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ShowLinesProperty =
            DependencyProperty.Register("ShowLines", typeof(bool), typeof(UniformBoard), new UIPropertyMetadata(true, OnDependencyPropertyChanged));


        #endregion

        #region LineBrush


        public Brush LineBrush
        {
            get { return (Brush)GetValue(SeparatorBrushProperty); }
            set { SetValue(SeparatorBrushProperty, value); }
        }

        // Using a DependencyProperty as the backing store for LineBrush.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty SeparatorBrushProperty =
            DependencyProperty.Register("LineBrush", typeof(Brush), typeof(UniformBoard),
                                        new UIPropertyMetadata(Brushes.Black, OnDependencyPropertyChanged));


        #endregion

        #region Rows


        public int Rows
        {
            get { return (int)GetValue(RowsProperty); }
            set { SetValue(RowsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Rows.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty RowsProperty =
            DependencyProperty.Register("Rows", typeof(int), typeof(UniformBoard), new UIPropertyMetadata(9, OnDependencyPropertyChanged));


        #endregion

        #region Columns

        public int Columns
        {
            get { return (int)GetValue(ColumnsProperty); }
            set { SetValue(ColumnsProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Columns.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty ColumnsProperty =
            DependencyProperty.Register("Columns", typeof(int), typeof(UniformBoard), new UIPropertyMetadata(5, OnDependencyPropertyChanged));

        #endregion

        #region AnimationInMiliseconds

        // Using a DependencyProperty as the backing store for AnimationMilliseconds.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimationMillisecondsProperty =
            DependencyProperty.Register("AnimationMilliseconds", typeof(int), typeof(UniformBoard), new FrameworkPropertyMetadata(1250));

        public int AnimationMilliseconds
        {
            get { return (int)GetValue(AnimationMillisecondsProperty); }
            set { SetValue(AnimationMillisecondsProperty, value); }
        }
        #endregion

        #region Animate



        public bool Animate
        {
            get { return (bool)GetValue(AnimateProperty); }
            set { SetValue(AnimateProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Animate.  This enables animation, styling, binding, etc...
        public static readonly DependencyProperty AnimateProperty =
            DependencyProperty.Register("Animate", typeof(bool), typeof(UniformBoard), new UIPropertyMetadata(true, OnDependencyPropertyChanged));



        #endregion

        #endregion

        #region On Properties Changed

        private static void RefreshPanel(UIElement p)
        {
            p.InvalidateVisual();
            p.InvalidateMeasure();
            p.InvalidateArrange();
        }

        private static void OnAttachedPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformBoard panel = VisualTreeHelperUtil.GetParent(d, (parent) => parent is UniformBoard) as UniformBoard;
            if (panel != null)
                RefreshPanel(panel);
        }

        private static void OnDependencyPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UniformBoard panel = d as UniformBoard;
            if (panel != null)
                RefreshPanel(panel);
        }

        #endregion

        #region Animation

        private void AnimateAll(double columnWidth, double rowHeight)
        {
            //Apply exactly the same algorithm, but instide of Arrange a call AnimateTo method
            double x, y;
            Size s;
            foreach (UIElement child in Children)
            {
                int row = (int)child.GetValue(UniformBoard.RowProperty);
                int rowSpan = (int)child.GetValue(UniformBoard.RowSpanProperty);
                int column = (int)child.GetValue(UniformBoard.ColumnProperty);

                x = column * columnWidth;
                y = row * rowHeight;
                s = new Size(columnWidth, rowSpan * rowHeight);
                AnimateTo(child as UIElement, x, y, AnimationMilliseconds);
            }
        }


        private void AnimateTo(UIElement child, double x, double y, int duration)
        {
            TransformGroup group = (TransformGroup)child.RenderTransform;
            TranslateTransform trans = (TranslateTransform)group.Children[0];

            if (trans.X <= 0 && trans.Y <= 0)
                duration = 0;

            trans.BeginAnimation(TranslateTransform.XProperty, MakeAnimation(x, duration));
            trans.BeginAnimation(TranslateTransform.YProperty, MakeAnimation(y, duration));
        }

        private DoubleAnimation MakeAnimation(double to, int duration)
        {
            DoubleAnimation anim = new DoubleAnimation(to, TimeSpan.FromMilliseconds(duration));
            anim.AccelerationRatio = 0.2;
            anim.DecelerationRatio = 0.7;
            return anim;
        }

        #endregion

        protected override Size MeasureOverride(Size availableSize)
        {
            double columnWidth = availableSize.Width / Columns; //Here we have the width for each column
            double rowHeight = availableSize.Height / Rows;

            foreach (UIElement child in Children)
            {
                int rowSpan = (int)child.GetValue(UniformBoard.RowSpanProperty);
                child.Measure(new Size(columnWidth, rowHeight * rowSpan));   
            }

            return new Size(Columns * columnWidth, rowHeight * Rows);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            double columnWidth = finalSize.Width / Columns; //Here we have the width for each column
            double rowHeight = finalSize.Height / Rows;
            if (!Animate)
            {
                //if is not animated then loacte items with Arrange
                double x, y;
                Size s;
                foreach (UIElement child in Children)
                {
                    child.RenderTransform = null;
                    int row = (int) child.GetValue(UniformBoard.RowProperty);
                    int rowSpan = (int) child.GetValue(UniformBoard.RowSpanProperty);
                    int column = (int) child.GetValue(UniformBoard.ColumnProperty);

                    x = column*columnWidth;
                    y = row*rowHeight;
                    s = new Size(columnWidth, rowSpan*rowHeight);
                    child.Arrange(new Rect(new Point(x, y), s));
                }
            }
            else
            {	//if is animated then arrange elements to 0,0, and then put them on its location using the transform
                foreach (UIElement child in InternalChildren)
                {
                    int rowSpan = (int)child.GetValue(UniformBoard.RowSpanProperty);
                    // If this is the first time we've seen this child, add our transforms
                    if (child.RenderTransform as TransformGroup == null)
                    {
                        child.RenderTransformOrigin = new Point(0.5, 0.5);
                        TransformGroup group = new TransformGroup();
                        child.RenderTransform = group;
                        group.Children.Add(new TranslateTransform());
                    }
                    //locate all children in 0,0 point
                    var s = new Size(columnWidth, rowSpan * rowHeight);
                    child.Arrange(new Rect(new Point(0, 0), s));
                }
                AnimateAll(columnWidth, rowHeight);
            }

            return new Size(Columns * columnWidth, Rows * rowHeight);
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
            if (LinesThickness <= 0 || !ShowLines)
                return;

            double width = ActualWidth / Columns;
            double xToDraw = 0;
            for (int i = 0; i < Columns; i++)
            {
                dc.DrawLine(new Pen(LineBrush, LinesThickness), new Point(xToDraw, 0), new Point(xToDraw, ActualHeight));
                xToDraw += width;
                //xToDraw += LinesThickness;
            }
            //draw last x line
            dc.DrawLine(new Pen(LineBrush, LinesThickness), new Point(xToDraw, 0), new Point(xToDraw, ActualHeight));

            double height = ActualHeight / Rows;
            double yToDraw = 0;
            for (int i = 0; i < Rows; i++)
            {
                dc.DrawLine(new Pen(LineBrush, LinesThickness), new Point(0, yToDraw), new Point(ActualWidth, yToDraw));
                yToDraw += height;
                //yToDraw += LinesThickness;
            }
            //draw last y line
            dc.DrawLine(new Pen(LineBrush, LinesThickness), new Point(0, yToDraw), new Point(ActualWidth, yToDraw));
        }
    }
}
