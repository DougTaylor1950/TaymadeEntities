//-----------------------------------------------------------------------
// <copyright file="FFMpegSupport.cs" company="Taymade Software Services">
//     Copyright (c) Taymade Software Services. All rights reserved.
// </copyright>
// <created>19/01/2020 21:07:02 19/01/2020 21:07:02 </created>
// <author>Doug Taylor</author>
//-----------------------------------------------------------------------

namespace TaymadeEntities.Support
{
    using TaymadeEntities.Models;
    using ReactiveUI;
    using System;
    using System.Drawing;
    using Newtonsoft.Json;

    /// <summary>
    /// Defines the <see cref="GammaCorrections" />.
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    public class GammaCorrections : ModelBase
    {
        #region Fields

        /// <summary>
        /// The clip.
        /// </summary>
        public bool clip = false;

        /// <summary>
        /// Defines the ClipRectangle.
        /// </summary>
        [JsonIgnore]
        public Rectangle ClipRectangle;

        /// <summary>
        /// The crop.
        /// </summary>
        public bool crop = false;

        /// <summary>
        /// Defines the blue.
        /// </summary>
        private double blue = 1.0;

        /// <summary>
        /// Defines the brightness.
        /// </summary>
        private double brightness = 0.0;

        /// <summary>
        /// Defines the contrast.
        /// </summary>
        private double contrast = 1.0;

        /// <summary>
        /// Defines the gamma.
        /// </summary>
        private double gamma = 1.0;

        /// <summary>
        /// Defines the green.
        /// </summary>
        private double green = 1.0;

        /// <summary>
        /// Defines the red.
        /// </summary>
        private double red = 1.0;

        /// <summary>
        /// Defines the saturation.
        /// </summary>
        private double saturation = 1.0;

        /// <summary>
        /// The scale.
        /// </summary>
        private bool scale = false;

        /// <summary>
        /// Defines the sharpness.
        /// </summary>
        private double sharpness = 0.0;

        /// <summary>
        /// Defines the start.
        /// </summary>
        private TimeSpan start;

        /// <summary>
        /// Defines the until.
        /// </summary>
        private TimeSpan until;

        /// <summary>
        /// Defines the weight.
        /// </summary>
        private double weight = 1.0;
        private double left;
        private double top;
        private double width;
        private double height;
        private bool correct = false;
        private int scaleHeight = 720;
        private int scaleWidth;
        private double scaleFactor = 1.0;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the Blue.
        /// </summary>
        [JsonProperty]
        public double Blue { get => blue; set => blue = value; }

        public bool ShowOrProcess { get; set; }

        public bool Clip { get => clip; set => this.RaiseAndSetIfChanged(ref clip, value); }
        [JsonProperty]
        public bool Crop
        {
            get => crop;
            set => this.RaiseAndSetIfChanged(ref crop, value);
        }

        [JsonProperty]
        public bool Correct { get => correct; set => this.RaiseAndSetIfChanged(ref correct, value); }

        /// <summary>
        /// Gets or sets the Brightness.
        /// </summary>
        [JsonProperty]
        public double Brightness { get => brightness; set => this.RaiseAndSetIfChanged(ref brightness, value); }

        /// <summary>
        /// Gets or sets the Contrast.
        /// </summary>
        [JsonProperty]
        public double Contrast { get => contrast; set => this.RaiseAndSetIfChanged(ref contrast, value); }

        /// <summary>
        /// Gets or sets the Gamma.
        /// </summary>
        [JsonProperty]
        public double Gamma { get => gamma; set => this.RaiseAndSetIfChanged( ref gamma, value); }

        /// <summary>
        /// Gets or sets the Green.
        /// </summary>
        [JsonProperty]
        public double Green { get => green; set => this.RaiseAndSetIfChanged(ref green, value); }

        /// <summary>
        /// Gets or sets the Red.
        /// </summary>
        [JsonProperty]
        public double Red { get => red; set => this.RaiseAndSetIfChanged(ref red, value); }

        /// <summary>
        /// Gets or sets the Saturation.
        /// </summary>
        [JsonProperty]
        public double Saturation { get => saturation; set => this.RaiseAndSetIfChanged(ref saturation, value); }

        public bool IsVideo { get; set; } = true;
        /// <summary>
        /// Gets or sets a value indicating whether this <see cref="GammaCorrections"/> is scale..
        /// </summary>
        [JsonProperty]
        public bool Scale { get => scale; set => this.RaiseAndSetIfChanged(ref scale, value); }

        public int ScaleHeight { get => scaleHeight; set => this.RaiseAndSetIfChanged(ref scaleHeight, value); }

        public double ScaleFactor { get => scaleFactor; set => this.RaiseAndSetIfChanged(ref scaleFactor, value); }


        public int ScaleWidth { get => scaleWidth; set => this.RaiseAndSetIfChanged(ref scaleWidth, value); }

        /// <summary>
        /// Gets or sets the Sharpness.
        /// </summary>
        [JsonProperty]
        public double Sharpness { get => sharpness; set => this.RaiseAndSetIfChanged(ref sharpness, value); }

        /// <summary>
        /// Gets or sets the Start.
        /// </summary>
        [JsonIgnore]
        public TimeSpan Start { get => start; set => this.RaiseAndSetIfChanged(ref start, value); }

        /// <summary>
        /// Gets or sets the Until.
        /// </summary>
        [JsonIgnore]
        public TimeSpan Until { get => until; set => this.RaiseAndSetIfChanged(ref until, value); }

        /// <summary>
        /// Gets or sets the Weight.
        /// </summary>
        public double Weight { get => weight; set => this.RaiseAndSetIfChanged(ref weight, value); }

        public double Left
        {
            get
            {
                //left = ClipRectangle.X;
                return left;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref left, value);
                ClipRectangle.X = (int)Left;
            }
        }

        public double Top
        {
            get
            {
                //top = ClipRectangle.Y;
                return top;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref top, value);
                ClipRectangle.Y = (int)top;
            }
        }

        public double Width
        {
            get
            {
                // width = ClipRectangle.Width;
                return width;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref width, value);
                ClipRectangle.Width = (int)width;
            }
        }

        public double Height
        {
            get
            {
                //height = ClipRectangle.Height;
                return height;
            }

            set
            {
                this.RaiseAndSetIfChanged(ref height, value);
                ClipRectangle.Height = (int)height;
                ScaleHeight = (int)height;
                
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The GammaCorrectionString.
        /// </summary>
        /// <returns>The <see cref="string"/>.</returns>
        public string GammaCorrectionString()
        {
            string gammaCorrect = "";
            if (Correct)
            {
                gammaCorrect = " -vf eq=gamma=" + gamma.ToString().Trim()
                       + ":contrast=" + contrast.ToString().Trim()
                       + ":brightness=" + brightness.ToString().Trim()
                       + ":saturation=" + saturation.ToString().Trim()
                       + ":gamma_r=" + Red.ToString().Trim()
                       + ":gamma_b=" + Blue.ToString().Trim()
                       + ":gamma_g=" + Green.ToString().Trim()
                       + ":gamma_weight=" + Weight.ToString().Trim();

                if (sharpness > 0)
                {
                    gammaCorrect += ",unsharp=5:5:" + sharpness.ToString().Trim() + ":5:5:0.0 ";
                }

                if (Crop && IsVideo)
                {
                    gammaCorrect = gammaCorrect.Trim() + ",crop=" + ClipRectangle.Width.ToString().Trim() + ":"
                        + ClipRectangle.Height.ToString().Trim() + ":"
                        + ClipRectangle.X.ToString().Trim() + ":"
                        + ClipRectangle.Y.ToString().Trim() + " ";

                    if (Scale && IsVideo)
                    {
                        gammaCorrect = gammaCorrect.Trim() + ",scale=" + ScaleWidth.ToString().Trim() + ":" + ScaleHeight.ToString().Trim()
                        + " ";
                    }
                } else if (Scale && IsVideo)
                    gammaCorrect = gammaCorrect.Trim() + ",scale=" + ScaleWidth.ToString().Trim() + ":" + ScaleHeight.ToString().Trim()
                        + " ";

                if (Clip && IsVideo)
                {
                    if (!ShowOrProcess)
                        gammaCorrect += " -ss " + Start.ToString() + " -t " + Until.ToString() + " ";
                    else
                        gammaCorrect += " -ss " + Start.ToString() + " -to " + Until.ToString() + " ";
                }
            }
            else if (IsVideo)
            {
                if (Clip)
                {
                    if (!ShowOrProcess)
                        gammaCorrect = " -ss " + Start.ToString() + " -t " + Until.ToString() + " ";
                    else
                        gammaCorrect = " -ss " + Start.ToString() + " -to " + Until.ToString() + " ";

                }
                if (Scale)
                {
                    gammaCorrect = gammaCorrect.Trim() + ",scale=" + ScaleWidth.ToString().Trim() + ":" + ScaleHeight.ToString().Trim()
                    + " ";
                }
            }

           

            return gammaCorrect;
        }

        /// <summary>
        /// The Reset.
        /// </summary>
        public void Reset()
        {
            gamma = 1.0;
            brightness = 0.0;
            saturation = 1.0;
            sharpness = 0.0;
            contrast = 1.0;
            red = 1.0;
            blue = 1.0;
            green = 1.0;
            weight = 1.0;
            ClipRectangle = new Rectangle();
        }

        public void Save(string configPath)
        {
            // convert object to json and save as movie name +config.json
            string json = JsonConvert.SerializeObject(this, Formatting.Indented, new JsonSerializerSettings
            {
                NullValueHandling = NullValueHandling.Ignore
            });
            System.IO.File.WriteAllText(configPath, json);

        }

        public void Load(string configPath)
        {
            if (System.IO.File.Exists(configPath))
            {
                string json = System.IO.File.ReadAllText(configPath);
                GammaCorrections? loaded = JsonConvert.DeserializeObject<GammaCorrections>(json);
                if (loaded != null)
                {
                    this.Blue = loaded.Blue;
                    this.Brightness = loaded.Brightness;
                    this.Contrast = loaded.Contrast;
                    this.Crop = loaded.Crop;
                    this.Correct = loaded.Correct;
                    this.Gamma = loaded.Gamma;
                    this.Green = loaded.Green;
                    this.Red = loaded.Red;
                    this.Saturation = loaded.Saturation;
                    this.Scale = loaded.Scale;
                    this.ScaleHeight = loaded.ScaleHeight;
                    this.ScaleWidth = loaded.ScaleWidth;
                    this.Sharpness = loaded.Sharpness;
                    this.Start = loaded.Start;
                    this.Until = loaded.Until;
                    this.Weight = loaded.Weight;
                    this.ClipRectangle = loaded.ClipRectangle;
                    this.Left = loaded.Left;
                    this.Top = loaded.Top;
                    this.Width = loaded.Width;
                    this.Height = loaded.Height;
                }
            }
        }

        #endregion
    }
}
