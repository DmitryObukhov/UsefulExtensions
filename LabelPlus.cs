/// <summary>
/// Label control with built-in effects like AutoFit, MouseOver, Shadow, Outer Glow, and Pulse Glow
/// http://www.codeproject.com/Articles/95399/gLabel-Custom-Label-with-Special-Effects-VB-NET
/// </summary>
/// <remarks>
/// v1.0.6
/// 01-26-2011
/// Added Padding Support
/// Added TextWordWrap Property
/// Added AutoEllispis Support
/// v1.0.7
/// 03-03-2011
/// Added MouseOverForeColor Property
/// Added Checked Property
/// Added Checked Color Property
/// Added Checked Toggle State Method
/// v1.0.8
/// 01-03-2012
/// Added BlendItemsConverter
/// Cleaned up Default Values
/// Added CheckedType Property
/// </remarks>
/// 


using System.Drawing.Drawing2D;
using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using System.Windows.Forms;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UsefulExtensions
{

    [System.Diagnostics.DebuggerStepThrough()]
    public partial class LabelPlus : Label
    {
        private Timer Pulser = new Timer();
        private bool _MouseIsOver;

        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            Pulser.Tick += new System.EventHandler(Pulser_Tick);
        }

        #endregion


        public LabelPlus()
        {
            // VBConversions Note: Non-static class variable initialization is below.  Class variables cannot be initially assigned non-static values in C#.
            _MouseOverColor = Color.Crimson;
            _MouseOverForeColor = Color.MediumBlue;
            _CheckedColor = Color.Crimson;
            _ShadowColor = Color.Gray;
            _GlowColor = Color.CornflowerBlue;
            _BorderColor = Color.Black;

            InitializeComponent(); // This call is required by the Windows Form Designer.
                                   // Add any initialization after the InitializeComponent() call.
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.OptimizedDoubleBuffer, true);

            ForeColor = Color.MediumBlue;
            Font = new Font("Arial", 12, Font.Style);
            PulseSpeed = 25;
            Size = new Size(75, 21);
            TextAlign = ContentAlignment.MiddleCenter;
        }

        [DefaultValue(typeof(Font), "Arial, 12pt")]
        public override Font Font
        {
            get
            {
                return (base.Font);
            }
            set
            {
                base.Font = value;
            }
        }

        [DefaultValue(typeof(Color), "MediumBlue")]
        public override Color ForeColor
        {
            get
            {
                return (base.ForeColor);
            }
            set
            {
                base.ForeColor = value;
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public override bool AutoSize
        {
            get
            {
                return base.AutoSize;
            }
            set
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool Image
        {
            get
            {
                return false; //always false
            }
            set //empty
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new BorderStyle BorderStyle
        {
            get
            {
                return BorderStyle.None; //always false
            }
            set //None
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool ImageAlign
        {
            get
            {
                return false; //always false
            }
            set //empty
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool ImageIndex
        {
            get
            {
                return false; //always false
            }
            set //empty
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool ImageKey
        {
            get
            {
                return false; //always false
            }
            set //empty
            {
            }
        }

        [Browsable(false)]
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public new bool ImageList
        {
            get
            {
                return false; //always false
            }
            set //empty
            {
            }
        }

        private bool _MouseOver;
        /// <summary>
        /// Get or Set if the gLabel will Glow when the mouse is over it
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the gLabel will Glow when the mouse is over it")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(false)]
        public bool MouseOver
        {
            get
            {
                return _MouseOver;
            }
            set
            {
                _MouseOver = value;
            }
        }

        private Color _MouseOverColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
                                       /// <summary>
                                       /// Get or Set what color the gLabel will Glow when the mouse is over it
                                       /// </summary>
        [Category("Appearance")]
        [Description("Get or Set what color the gLabel will Glow when the mouse is over it")]
        [DefaultValue(typeof(Color), "Crimson")]
        public Color MouseOverColor
        {
            get
            {
                return _MouseOverColor;
            }
            set
            {
                _MouseOverColor = value;
                Invalidate();
            }
        }

        private Color _MouseOverForeColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
                                           /// <summary>
                                           /// Get or Set what Forecolor the gLabel will be when the mouse is over it
                                           /// </summary>
        [Category("Appearance")]
        [Description("Get or Set what Forecolor the gLabel will be when the mouse is over it")]
        [DefaultValue(typeof(Color), "MediumBlue")]
        public Color MouseOverForeColor
        {
            get
            {
                return _MouseOverForeColor;
            }
            set
            {
                _MouseOverForeColor = value;
                Invalidate();
            }
        }

        private bool _Checked;
        /// <summary>
        /// Get or Set the Checked status
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set the Checked status")]
        [DefaultValue(false)]
        public bool Checked
        {
            get
            {
                return _Checked;
            }
            set
            {
                if (_CheckedType == eCheckedType.Radio)
                {
                    if (_Checked == false)
                    {

                        if (value)
                        {

                            foreach (Control ctrl in Parent.Controls)
                            {
                                if (ctrl.GetType() == typeof(LabelPlus))
                                {
                                    LabelPlus tgLabel = (LabelPlus)ctrl;
                                    if (tgLabel.CheckedType == eCheckedType.Radio && tgLabel.Checked)
                                    {
                                        tgLabel.Checked = false;
                                    }
                                }
                            }
                        }
                    }
                }
                if (_GlowMatchChecked)
                {
                    _GlowState = value;
                }
                _Checked = value;
                Invalidate();
            }
        }

        public void ToggleChecked()
        {
            @Checked = !@Checked;
        }

        public enum eCheckedType
        {
            Label,
            Check,
            Radio
        }
        private eCheckedType _CheckedType = eCheckedType.Label;
        /// <summary>
        /// Get or Set the Checked behavior
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set the Checked behavior")]
        [DefaultValue(typeof(eCheckedType), "Label")]
        public eCheckedType CheckedType
        {
            get
            {
                return _CheckedType;
            }
            set
            {
                _CheckedType = value;
                Invalidate();
            }
        }

        private Color _CheckedColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
                                     /// <summary>
                                     /// Get or Set what color the gLabel will Glow when the mouse is over it
                                     /// </summary>
        [Category("Appearance")]
        [Description("Get or Set what color the gLabel will Glow when the mouse is over it")]
        [DefaultValue(typeof(Color), "Crimson")]
        public Color CheckedColor
        {
            get
            {
                return _CheckedColor;
            }
            set
            {
                _CheckedColor = value;
                Invalidate();
            }
        }

        private bool _FeatherState = true;
        /// <summary>
        /// Get or Set if the text is glowing
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the glow is feathered")]
        [DefaultValue(true)]
        public bool FeatherState
        {
            get
            {
                return _FeatherState;
            }
            set
            {
                _FeatherState = value;
                Invalidate();
            }
        }

        private int _Feather = 100;
        /// <summary>
        /// Get or Set the level of feathering (blurring) of the Outer Glow
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set the level of feathering (blurring) of the Outer Glow")]
        [DefaultValue(100)]
        public int Feather
        {
            get
            {
                return _Feather;
            }
            set
            {
                _Feather = value;
                if (_Feather > 255)
                {
                    _Feather = 255;
                }
                if (_Feather < 0)
                {
                    _Feather = 0;
                }
                Invalidate();
                PulseDirection = _Feather / 25;
                if (PulseDirection < 0)
                {
                    PulseDirection = 1;
                }
            }
        }

        private int _PulseAdj;
        private bool _Pulse;
        /// <summary>
        /// Get or Set if the gLabel should be Pulsing or not
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the gLabel should be Pulsing or not")]
        [RefreshProperties(RefreshProperties.Repaint)]
        [DefaultValue(false)]
        public bool Pulse
        {
            get
            {
                return _Pulse;
            }
            set
            {
                _Pulse = value;
                if (value)
                {
                    _PulseAdj = 0;
                    Pulser.Start();
                }
                else
                {
                    Pulser.Stop();
                    _PulseAdj = 0;
                    Invalidate();
                    //PulseDirection = _Feather \ 25
                    //If PulseDirection < 0 Then PulseDirection = 1
                }
            }
        }

        /// <summary>
        /// Get or Set how fast to pulse the gLabel
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set how fast to pulse the gLabel")]
        [DefaultValue(25)]
        public int PulseSpeed
        {
            get
            {
                return Pulser.Interval;

            }
            set
            {
                Pulser.Interval = value;
            }
        }

        private Point _ShadowOffset = new Point(5, 5);
        /// <summary>
        /// Get or Set how far to offset the shadow from the text
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set how far to offset the shadow from the text")]
        [DefaultValue(typeof(Point), "5,5")]
        public Point ShadowOffset
        {
            get
            {
                return _ShadowOffset;
            }
            set
            {
                _ShadowOffset = value;
                Invalidate();
            }
        }

        private bool _ShadowState;
        /// <summary>
        /// Get or Set if the gLabel displays the shadow text
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the gLabel displays the shadow text")]
        [DefaultValue(false)]
        public bool ShadowState
        {
            get
            {
                return _ShadowState;
            }
            set
            {
                _ShadowState = value;
                Invalidate();
            }
        }

        private Color _ShadowColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
                                    /// <summary>
                                    /// Get or Set what color to use for the shadow text
                                    /// </summary>
        [Category("Appearance")]
        [Description("Get or Set what color to use for the shadow text")]
        [DefaultValue(typeof(Color), "Gray")]
        public Color ShadowColor
        {
            get
            {
                return _ShadowColor;
            }
            set
            {
                _ShadowColor = value;
                Invalidate();
            }
        }

        private bool _GlowMatchChecked = true;
        /// <summary>
        /// Get or Set if the text is glowing
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the GlowState auto matches the Checked Value")]
        [DefaultValue(true)]
        public bool GlowMatchChecked
        {
            get
            {
                return _GlowMatchChecked;
            }
            set
            {
                _GlowMatchChecked = value;
                Invalidate();
            }
        }

        private bool _GlowState = true;
        /// <summary>
        /// Get or Set if the text is glowing
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the text is glowing")]
        [DefaultValue(true)]
        public bool GlowState
        {
            get
            {
                return _GlowState;
            }
            set
            {
                _GlowState = value;
                Invalidate();
            }
        }

        private int _Glow = 8;
        /// <summary>
        /// Get or Set how far out the text glows
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set how far out the text glows")]
        [DefaultValue(8)]
        public int Glow
        {
            get
            {
                return _Glow;
            }
            set
            {
                _Glow = value;
                Invalidate();
            }
        }

        private Color _GlowColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
                                  /// <summary>
                                  /// Get or Set what color to use for the Glow
                                  /// </summary>
        [Category("Appearance")]
        [Description("Get or Set what color to use for the Glow")]
        [DefaultValue(typeof(Color), "CornflowerBlue")]
        public Color GlowColor
        {
            get
            {
                return _GlowColor;
            }
            set
            {
                _GlowColor = value;
                Invalidate();
            }
        }

        public enum eBorder
        {
            All,
            Top,
            Bottom

        }

        private AnchorStyles _Border = AnchorStyles.None;
        /// <summary>
        /// Get or Set to show or not show the border
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set to show or not show the border")]
        [DefaultValue(typeof(AnchorStyles), "None")]
        public AnchorStyles Border
        {
            get
            {
                return _Border;
            }
            set
            {
                _Border = value;
                Invalidate();
            }
        }

        private Color _BorderColor; // VBConversions Note: Initial value cannot be assigned here since it is non-static.  Assignment has been moved to the class constructors.
                                    /// <summary>
                                    /// Get or Set what Color to draw the border
                                    /// </summary>
        [Category("Appearance")]
        [Description("Get or Set what Color to draw the border")]
        [DefaultValue(typeof(Color), "Black")]
        public Color BorderColor
        {
            get
            {
                return _BorderColor;
            }
            set
            {
                _BorderColor = value;
                Invalidate();
            }
        }

        private float _BorderWidth = 1;
        /// <summary>
        /// Get or Set what Width to draw the border
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set what Width to draw the border")]
        [DefaultValue(typeof(Single), "1")]
        public float BorderWidth
        {
            get
            {
                return _BorderWidth;
            }
            set
            {
                _BorderWidth = value;
                Invalidate();
            }
        }

        private bool _AutoFit;
        /// <summary>
        /// Get or Set if the text is fitted to the DisplayRectangle or uses Font Size
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the text is fitted to the DisplayRectangle or uses Font Size")]
        [DefaultValue(false)]
        public bool AutoFit
        {
            get
            {
                return _AutoFit;
            }
            set
            {
                _AutoFit = value;
                Invalidate();
            }
        }

        private bool _textWordWrap = true;
        /// <summary>
        /// Get or Set if the text wraps inside the DisplayRectangle
        /// </summary>
        [Category("Appearance")]
        [Description("Get or Set if the text wraps inside the DisplayRectangle")]
        [DefaultValue(true)]
        public bool TextWordWrap
        {
            get
            {
                return _textWordWrap;
            }
            set
            {
                _textWordWrap = value;
                if (value)
                {
                    sf.FormatFlags = 0; // null;
                }
                else
                {
                    sf.FormatFlags = StringFormatFlags.NoWrap;
                }
                Invalidate();
            }
        }

        /// <summary>
        ///  Get or Set if the text adds an Ellipsis (...) when the text exceeds the width
        /// </summary>
        public new bool AutoEllipsis
        {
            get
            {
                return base.AutoEllipsis;
            }
            set
            {
                base.AutoEllipsis = value;
                if (value)
                {
                    sf.Trimming = StringTrimming.EllipsisCharacter;
                }
                else
                {
                    sf.Trimming = StringTrimming.None;
                }
                Invalidate();
            }
        }

        private StringFormat sf = new StringFormat();
        [DefaultValue(typeof(ContentAlignment), "MiddleCenter")]
        public override ContentAlignment TextAlign
        {
            get
            {
                return base.TextAlign;
            }
            set
            {
                base.TextAlign = value;
                switch (TextAlign)
                {
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.BottomRight:
                        sf.LineAlignment = StringAlignment.Far;
                        break;
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.MiddleRight:
                        sf.LineAlignment = StringAlignment.Center;
                        break;
                    case ContentAlignment.TopCenter:
                    case ContentAlignment.TopLeft:
                    case ContentAlignment.TopRight:
                        sf.LineAlignment = StringAlignment.Near;
                        break;
                }
                switch (TextAlign)
                {
                    case ContentAlignment.BottomRight:
                    case ContentAlignment.MiddleRight:
                    case ContentAlignment.TopRight:
                        sf.Alignment = StringAlignment.Far;
                        break;
                    case ContentAlignment.BottomCenter:
                    case ContentAlignment.MiddleCenter:
                    case ContentAlignment.TopCenter:
                        sf.Alignment = StringAlignment.Center;
                        break;
                    case ContentAlignment.BottomLeft:
                    case ContentAlignment.MiddleLeft:
                    case ContentAlignment.TopLeft:
                        sf.Alignment = StringAlignment.Near;
                        break;
                }

            }
        }

        public enum eFillType
        {
            Solid,
            GradientLinear
        }

        private eFillType _FillType = eFillType.Solid;
        /// <summary>
        /// The eFillType Fill Type to apply to the gLabel
        /// </summary>
        [Description("The Fill Type to apply to the gLabel")]
        [Category("Appearance")]
        [DefaultValue(typeof(eFillType), "Solid")]
        public eFillType FillType
        {
            get
            {
                return _FillType;
            }
            set
            {
                _FillType = value;
                Invalidate();
            }
        }

        private LinearGradientMode _FillTypeLinear = LinearGradientMode.Vertical;
        /// <summary>
        /// The Linear Blend type
        /// </summary>
        [Description("The Linear Blend type"), Category("Appearance")]
        [DefaultValue(typeof(LinearGradientMode), "Vertical")]
        public LinearGradientMode FillTypeLinear
        {
            get
            {
                return _FillTypeLinear;
            }
            set
            {
                _FillTypeLinear = value;
                Invalidate();
            }
        }

        //Private _ForeColorBlend As cBlendItems = DefaultColorFillBlend()
        ///' <summary>
        ///' The ColorBlend used to fill the gLabel
        ///' </summary>
        //<Description("The ColorBlend used to fill the gLabel"),
        //Category("Appearance"),
        //RefreshProperties(RefreshProperties.All),
        //Editor(GetType(BlendTypeEditor), GetType(UITypeEditor))>
        //Public Property ForeColorBlend() As cBlendItems
        //    Get
        //        Return _ForeColorBlend
        //    End Get
        //    Set(ByVal value As cBlendItems)
        //        _ForeColorBlend = value
        //        Invalidate()
        //    End Set
        //End Property

        private cBlendItems DefaultColorFillBlend()
        {
            return new cBlendItems(new Color[] { Color.AliceBlue, Color.RoyalBlue, Color.Navy }, new Single[] { 0, 0.5F, 1 });
        }

        //'The standard <DefaultValue(XXX)> attribute
        //'will not work correctly for custom Types
        //'These two Methods are needed to set the Default Value Correctly
        //Public Sub ResetForeColorBlend()
        //    ForeColorBlend = DefaultColorFillBlend()
        //End Sub

        //Private Function ShouldSerializeForeColorBlend() As Boolean
        //    Return Not (_ForeColorBlend.Equals(DefaultColorFillBlend))
        //End Function

        protected override void OnPaintBackground(PaintEventArgs pevent)
        {
            if (BackColor == Color.Transparent)
            {
                base.OnPaintBackground(pevent);
            }
            else
            {
                pevent.Graphics.Clear(BackColor);
                //pevent.Graphics.Clear(EnabledColor(BackColor))
            }
        }

        protected override void OnPaint(PaintEventArgs e)
        {
            //Setup Graphics
            Graphics g = e.Graphics;

            g.SmoothingMode = SmoothingMode.AntiAlias;
            //g.InterpolationMode = InterpolationMode.HighQualityBicubic

            //Paint Border if Requested
            if (_Border != AnchorStyles.None)
            {
                using (Pen pn = new Pen(EnabledColor(_BorderColor), _BorderWidth * 2))
                {
                    g.ResetTransform();
                    if (_Border == ((AnchorStyles)(AnchorStyles.Bottom | AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right)))
                    {
                        g.DrawRectangle(pn, 0, 0, Width - 1, Height - 1);
                    }
                    else
                    {
                        if (_Border == ((AnchorStyles)(_Border | AnchorStyles.Bottom)))
                        {
                            g.DrawLine(pn, 0, Height - 1, Width - 1, Height - 1);
                        }
                        if (_Border == ((AnchorStyles)(_Border | AnchorStyles.Top)))
                        {
                            g.DrawLine(pn, 0, 0, Width - 1, 0);
                        }
                        if (_Border == ((AnchorStyles)(_Border | AnchorStyles.Left)))
                        {
                            g.DrawLine(pn, 0, 0, 0, Height - 1);
                        }
                        if (_Border == ((AnchorStyles)(_Border | AnchorStyles.Right)))
                        {
                            g.DrawLine(pn, Width - 1, 0, Width - 1, Height - 1);
                        }

                    }

                }


            }
            //Paint Shadow if Requested
            if (_ShadowState)
            {
                using (GraphicsPath shadowpath = new GraphicsPath())
                {

                    MakeTextPath(shadowpath, g);

                    //Offset the Shadow
                    using (Matrix mx = new Matrix())
                    {
                        if (_AutoFit)
                        {
                            mx.Translate(_ShadowOffset.X - 2, _ShadowOffset.Y - 2);
                        }
                        else
                        {
                            mx.Translate(_ShadowOffset.X, _ShadowOffset.Y);
                        }
                        shadowpath.Transform(mx);
                    }


                    //Blur the edge a bit
                    int x = (int)(Fix((double)Height / 30) + 1);
                    for (int i = 1; i <= x; i++)
                    {
                        using (Pen pen = new Pen(EnabledColor(
                                Color.FromArgb((int)(200 - (((double)200 / x) * i)), _ShadowColor)), i))
                        {
                            pen.LineJoin = LineJoin.Round;
                            g.DrawPath(pen, shadowpath);
                        }

                    }
                    g.FillPath(new SolidBrush(EnabledColor(_ShadowColor)), shadowpath);
                }

            }

            using (GraphicsPath path = new GraphicsPath())
            {

                MakeTextPath(path, g);
                //Paint Glow if Requested
                Color gColor = _GlowColor;
                Color gForeColor = new Color();
                if (_Checked)
                {
                    gForeColor = _CheckedColor;
                }
                else
                {
                    gForeColor = ForeColor;
                }

                if (_GlowState || (_MouseOver && _MouseIsOver))
                {
                    if (_MouseOver && _MouseIsOver)
                    {
                        gColor = _MouseOverColor;
                        if (!_Checked)
                        {
                            gForeColor = _MouseOverForeColor;
                        }
                    }

                    if (_FeatherState)
                    {
                        for (int i = 1; i <= _Glow; i += 2)
                        {
                            int aGlow = (int)((_Feather - _PulseAdj) -
                                (((double)(_Feather - _PulseAdj) / _Glow) * i));
                            using (Pen pen = new Pen(Color.FromArgb(
                                    aGlow, EnabledColor(gColor)), i))
                            {
                                pen.LineJoin = LineJoin.Round;
                                g.DrawPath(pen, path);
                            }

                        }
                    }
                    else
                    {
                        using (Pen pen = new Pen(Color.FromArgb(
                                _Feather - _PulseAdj, EnabledColor(gColor)), _Glow))
                        {
                            pen.LineJoin = LineJoin.Round;
                            g.DrawPath(pen, path);
                        }

                    }
                }

                //Paint Label Text
                switch (_FillType)
                {
                    case eFillType.Solid:
                        path.FillMode = FillMode.Winding;
                        g.FillPath(new SolidBrush(EnabledColor(gForeColor)), path);
                        break;

                    case eFillType.GradientLinear:
                        using (LinearGradientBrush br = new LinearGradientBrush(
                                new RectangleF(path.GetBounds().X - 1, path.GetBounds().Y - 1,
                                path.GetBounds().Width + 2, path.GetBounds().Height + 2),
                                Color.White, Color.White, FillTypeLinear))
                        {
                            ColorBlend cb = new ColorBlend();
                            //cb.Colors = EnableBlends(_ForeColorBlend.iColor)
                            //cb.Positions = _ForeColorBlend.iPoint

                            br.InterpolationColors = cb;

                            g.FillPath(br, path);
                        }

                        break;

                }
            }

        }

        private int Fix(object o)
        {
            int i;
            decimal testD = 0;
            try
            {
                if (o.GetType() == testD.GetType())
                {
                    i = (int)o;
                    return i;
                }
                if (o == DBNull.Value)
                {
                    return 0;
                }
                else
                {
                    i = (int)o;
                    return i;
                }

            }
            catch
            {
                return 0;
            }

        }


        private void MakeTextPath(GraphicsPath path, Graphics g)
        {

            if (_AutoFit)
            {
                try
                {
                    //Determine the outer margin for the text so there
                    //is enough room for the glow and shadow
                    int pad = 2;
                    if (GlowState)
                    {
                        pad += (int)(_Glow - ((double)_Glow / 2) + Fix((double)_Glow / 30) * 3);
                    }
                    if (Border != AnchorStyles.None)
                    {
                        pad += (int)(BorderWidth + 1);
                    }

                    //Add Padding
                    Padding TextMargin = System.Windows.Forms.Padding.Add(Padding, new Padding(pad));

                    if (ShadowState)
                    {
                        TextMargin.Right += _ShadowOffset.X;
                        TextMargin.Bottom += (int)(_ShadowOffset.Y + ((double)(_ShadowOffset.Y) / 3));
                    }

                    //Get a rectangle for the area to paint the text
                    Rectangle target = new Rectangle(
                        TextMargin.Left,
                        TextMargin.Top,
                        Math.Max(5, ClientSize.Width - TextMargin.Horizontal),
                        Math.Max(5, ClientSize.Height - TextMargin.Vertical));

                    //Get the points for the corners of the area
                    PointF[] target_pts = new PointF[] {
                            new PointF(target.Left, target.Top),
                            new PointF(target.Right, target.Top),
                            new PointF(target.Left, target.Bottom)};

                    //Make a GraphicsPath of the Text String
                    //close to the size it needs to be
                    path.AddString(Text,
                        Font.FontFamily, (System.Int32)Font.Style,
                        target.Height, new PointF(0, 0), sf);

                    //Get a rectangle for the path of the text
                    RectangleF text_rectf = path.GetBounds();

                    //Transform the Graphics Object with the Matrix
                    //to fit the path rectangle inside the target rectangle
                    g.Transform = new Matrix(text_rectf, target_pts);

                }
                catch (Exception)
                {

                }

            }
            else
            {
                //create a GraphicsPath of the text
                //Because the GraphicsPath does not match exactly with
                //Drawing a String normally, multiply the font Size by
                //1.26 to get a pretty close representation of the size.
                path.AddString(Text, Font.FontFamily, (System.Int32)Font.Style, (int)(Font.Size * 1.26),
                    new Rectangle(
                    ClientRectangle.X + Padding.Left,
                    ClientRectangle.Y + Padding.Top,
                    ClientRectangle.Width - Padding.Horizontal,
                    ClientRectangle.Height - Padding.Vertical),
                    sf);

            }

        }

        protected override void OnMouseEnter(EventArgs e)
        {
            base.OnMouseEnter(e);
            if (MouseOver)
            {
                _MouseIsOver = true;
                Invalidate();
            }
        }

        protected override void OnMouseLeave(EventArgs e)
        {
            base.OnMouseLeave(e);
            if (MouseOver)
            {
                _MouseIsOver = false;
                Invalidate();
            }
        }

        protected override void OnClick(EventArgs e)
        {
            base.OnClick(e);
            if (CheckedType != eCheckedType.Label)
            {
                if (!(CheckedType == eCheckedType.Radio & _Checked))
                {
                    ToggleChecked();
                }
            }
        }

        /// <summary>
        /// Convert color to gray if Disabled else return origional color
        /// </summary>
        /// <param name="ColorIn">Color to Check</param>
        private Color EnabledColor(Color ColorIn)
        {
            if (Enabled)
            {
                return ColorIn;
            }
            else
            {
                int gray = (int)(ColorIn.R * 0.3 + ColorIn.G * 0.59 + ColorIn.B * 0.11);
                return Color.FromArgb(ColorIn.A, gray, gray, gray);
            }
        }

        /// <summary>
        /// Convert colorblend to grayscale if Disabled else return origional colorblend
        /// </summary>
        /// <param name="ColorIn">Colorblend to Check</param>
        private Color[] EnableBlends(Color[] ColorIn)
        {

            if (Enabled)
            {
                return ColorIn;
            }
            else
            {
                List<Color> tcolor = new List<Color>();
                foreach (Color c in ColorIn)
                {
                    int gray = (int)(c.R * 0.3 + c.G * 0.59 + c.B * 0.11);
                    tcolor.Add(Color.FromArgb(c.A, gray, gray, gray));
                }
                return tcolor.ToArray();
            }
        }

        private int PulseDirection = 1;
        private void Pulser_Tick(object sender, EventArgs e)
        {
            _PulseAdj += PulseDirection;
            if (_Feather - _PulseAdj < 10 || _Feather - _PulseAdj > _Feather)
            {
                PulseDirection *= -1;
                _PulseAdj += PulseDirection;
            }
            Invalidate();
        }
    }

    [System.Diagnostics.DebuggerStepThrough()]
    [TypeConverter(typeof(BlendItemsConverter))]
    public class cBlendItems
    {

        public cBlendItems()
        {

        }

        public cBlendItems(Color[] Color, float[] Pt)
        {
            iColor = Color;
            iPoint = Pt;
        }

        private Color[] _iColor;
        [Description("The Color for the Point"),Category("Appearance")]
        public System.Drawing.Color[] iColor
        {
            get
            {
                return _iColor;
            }
            set
            {
                _iColor = value;
            }
        }

        private float[] _iPoint;
        [Description("The Color for the Point"),Category("Appearance")]
        public float[] iPoint
        {
            get
            {
                return _iPoint;
            }
            set
            {
                _iPoint = value;
            }
        }

        public override string ToString()
        {
            // build the string as "Color1;Color2;Color3|Pt1;Pt2;Pt3"
            ArrayList bColors = new ArrayList();
            ArrayList bPoints = new ArrayList();
            foreach (Color bColor in _iColor)
            {
                if (bColor.IsNamedColor)
                {
                    bColors.Add(bColor.Name);
                }
                else
                {
                    bColors.Add(string.Format("{0},{1},{2},{3}", bColor.A, bColor.R, bColor.G, bColor.B));
                }
            }
            foreach (float bPoint in _iPoint)
            {
                bPoints.Add(bPoint.ToString());
            }

            return string.Format("{0}|{1}", string.Join(";", bColors.ToArray()), string.Join(";", bPoints.ToArray()));
        }

        public override bool Equals(object obj)
        {
            cBlendItems eObj = (cBlendItems)obj;
            if (iColor.Length != eObj.iColor.Length || iPoint.Length != eObj.iPoint.Length)
            {
                return false;
            }
            else
            {
                for (int i = 0; i <= iColor.Length - 1; i++)
                {
                    if (iColor[i] != eObj.iColor[i] || iPoint[i] != eObj.iPoint[i])
                    {
                        return false;
                    }
                }
                return true;
            }

        }

        public override int GetHashCode()
        {
            return this.GetHashCode();
        }

    }

    [System.Diagnostics.DebuggerStepThrough()]
    internal class BlendItemsConverter : ExpandableObjectConverter
    {

        public override bool GetCreateInstanceSupported(ITypeDescriptorContext context)
        {
            return true;
        }

        public override object CreateInstance(ITypeDescriptorContext context, IDictionary propertyValues)
        {
            cBlendItems bItem = new cBlendItems();
            bItem.iColor = (Color[])(propertyValues["iColor"]);
            bItem.iPoint = (Single[])(propertyValues["iPoint"]);
            return bItem;
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }
            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            if (value is string)
            {
                try
                {
                    string[] s = Convert.ToString(value).Split('|');
                    List<Color> bColors = new List<Color>();
                    List<Single> bPoints = new List<Single>();

                    foreach (string cstring in (s[0]).Split(';'))
                    {
                        bColors.Add((Color)(TypeDescriptor.GetConverter(
                            typeof(Color)).ConvertFromString(cstring)));
                    }
                    foreach (string cstring in (s[1]).Split(';'))
                    {
                        bPoints.Add(System.Convert.ToSingle(TypeDescriptor.GetConverter(
                            typeof(Single)).ConvertFromString(cstring)));
                    }
                    if (!(bColors == null) && !(bPoints == null))
                    {
                        if (bColors.Count != bPoints.Count)
                        {
                            throw (new ArgumentException(string.Format("Can not convert \'{0}\' to type cBlendItem", (value).ToString())));
                        }
                        return new cBlendItems(bColors.ToArray(), bPoints.ToArray());
                    }
                }
                catch (Exception)
                {
                    throw (new ArgumentException(string.Format("Can not convert \'{0}\' to type cBlendItem", (value).ToString())));
                }
            }
            else
            {
                return new cBlendItems();
            }
            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is cBlendItems)
            {
                cBlendItems _BlendItems = (cBlendItems)value;
                // build the string as "Color1;Color2;Color3|Pt1;Pt2;Pt3"
                ArrayList bColors = new ArrayList();
                ArrayList bPoints = new ArrayList();
                foreach (Color bColor in _BlendItems.iColor)
                {
                    if (bColor.IsNamedColor)
                    {
                        bColors.Add(bColor.Name);
                    }
                    else
                    {
                        bColors.Add(string.Format("{0},{1},{2},{3}", bColor.A, bColor.R, bColor.G, bColor.B));
                    }
                }
                foreach (float bPoint in _BlendItems.iPoint)
                {
                    bPoints.Add(bPoint.ToString());
                }
                return string.Format("{0}|{1}", string.Join(";", bColors.ToArray()), string.Join(";", bPoints.ToArray()));
            }
            return base.ConvertTo(context, culture, value, destinationType);
        }
    } //CornerConverter Code
}
