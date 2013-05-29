﻿namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Devices.Geolocation;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
#endif
#if WINDOWS_PHONE
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Dynamic;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using System.Windows;
    using Windows.Devices.Geolocation;
#endif

    /// <summary>
    /// Shows a user interface that can be used to select Facebook places.
    /// </summary>
    public class PlacePicker : Picker<GraphPlace>
    {
        #region Default Property Values

        private const string DefaultAccessToken = "";
        private const string DefaultDisplayFields = "id,name,location,category,picture,were_here_count";
        private const bool DefaultDisplayProfilePictures = true;
        private const string DefaultSearchText = "";
        private const int DefaultRadiusInMeters = 1000;
        private const int DefaultResultsLimit = 100;
        private const bool DefaultTrackLocation = false;
        private const double DefaultLatitude = 51.494338;
        private const double DefaultLongitude = -0.176759;
        private static readonly Size DefaultPictureSize = new Size(50, 50);

        #endregion Default Property Values

        #region Member variables

        private Geolocator geoLocator;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the PlacePicker class.
        /// </summary>
        public PlacePicker()
        {
            this.DefaultStyleKey = typeof(PlacePicker);
        }

        #region Events

        /// <summary>
        /// Occurs whenever a new place is about to be added to the list.
        /// </summary>
        public event EventHandler<DataItemRetrievedEventArgs<GraphPlace>> PlaceRetrieved;

        /// <summary>
        /// Occurs when the list of places has finished loading.
        /// </summary>
        public event EventHandler<DataReadyEventArgs<GraphPlace>> LoadCompleted;

        /// <summary>
        /// Occurs whenever an error occurs while loading data.
        /// </summary>
        public event EventHandler<LoadFailedEventArgs> LoadFailed;

        #endregion Events

        #region Properties

        #region AccessToken

        /// <summary>
        /// Gets or sets the access token returned by the Facebook Login service.
        /// </summary>
        public string AccessToken
        {
            get { return (string)this.GetValue(AccessTokenProperty); }
            set { this.SetValue(AccessTokenProperty, value); }
        }

        /// <summary>
        /// Identifies the AccessToken dependency property.
        /// </summary>
        public static readonly DependencyProperty AccessTokenProperty =
            DependencyProperty.Register("AccessToken", typeof(string), typeof(PlacePicker), new PropertyMetadata(DefaultAccessToken, OnAccessTokenPropertyChanged));

        private static async void OnAccessTokenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            await placePicker.RefreshData();
        }

        #endregion AccessToken

        #region DisplayFields

        /// <summary>
        /// Gets or sets additional fields to fetch when requesting place data.
        /// </summary>
        /// <remarks>
        /// By default, the following data is retrieved for each place: TODO: to be completed.
        /// </remarks>
        public string DisplayFields
        {
            get { return (string)GetValue(DisplayFieldsProperty); }
            set { this.SetValue(DisplayFieldsProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayFields dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayFieldsProperty =
            DependencyProperty.Register("DisplayFields", typeof(string), typeof(PlacePicker), new PropertyMetadata(DefaultDisplayFields));

        #endregion DisplayFields

        #region DisplayProfilePictures

        /// <summary>
        /// Specifies whether profile pictures are displayed.
        /// </summary>
        public bool DisplayProfilePictures
        {
            get { return (bool)GetValue(DisplayProfilePicturesProperty); }
            set { this.SetValue(DisplayProfilePicturesProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayProfilePictures dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayProfilePicturesProperty =
            DependencyProperty.Register("DisplayProfilePictures", typeof(bool), typeof(PlacePicker), new PropertyMetadata(DefaultDisplayProfilePictures));

        #endregion DisplayProfilePictures

        #region PictureSize
        /// <summary>
        /// Gets or sets the size of the profile pictures displayed.
        /// </summary>
        public Size PictureSize
        {
            get { return (Size)GetValue(PictureSizeProperty); }
            set { this.SetValue(PictureSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the PictureSize dependency property.
        /// </summary>
        public static readonly DependencyProperty PictureSizeProperty =
            DependencyProperty.Register("PictureSize", typeof(Size), typeof(PlacePicker), new PropertyMetadata(DefaultPictureSize));

        #endregion PictureSize

        #region SearchText

        /// <summary>
        /// Gets or sets the search text to filter place data (e.g. Restaurant, Supermarket, Sports, etc...)
        /// </summary>
        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { this.SetValue(SearchTextProperty, value); }
        }

        /// <summary>
        /// Identifies the SearchText dependency property.
        /// </summary>
        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(PlacePicker), new PropertyMetadata(DefaultSearchText, OnSearchTextPropertyChanged));

        private static async void OnSearchTextPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            await placePicker.RefreshData();
        }

        #endregion SearchText

        #region RadiusInMeters

        /// <summary>
        /// Gets or sets the distance in meters from the search location for which results are returned.
        /// </summary>
        public int RadiusInMeters
        {
            get { return (int)GetValue(RadiusInMetersProperty); }
            set { this.SetValue(RadiusInMetersProperty, value); }
        }

        /// <summary>
        /// Identifies the RadiusInMeters dependency property.
        /// </summary>
        public static readonly DependencyProperty RadiusInMetersProperty =
            DependencyProperty.Register("RadiusInMeters", typeof(int), typeof(PlacePicker), new PropertyMetadata(DefaultRadiusInMeters, OnRadiusInMetersPropertyChanged));

        private static async void OnRadiusInMetersPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            await placePicker.RefreshData();
        }

        #endregion RadiusInMeters

        #region Latitude

        /// <summary>
        /// Gets or sets the latitude of the location around which to retrieve place data.
        /// </summary>
        public double Latitude
        {
            get { return (double)GetValue(LatitudeProperty); }
            set { this.SetValue(LatitudeProperty, value); }
        }

        /// <summary>
        /// Identifies the Latitude dependency property.
        /// </summary>
        public static readonly DependencyProperty LatitudeProperty =
            DependencyProperty.Register("Latitude", typeof(double), typeof(PlacePicker), new PropertyMetadata(DefaultLatitude, OnLatitudePropertyChanged));

        private static async void OnLatitudePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            await placePicker.RefreshData();
        }
        
        #endregion Latitude

        #region Longitude

        /// <summary>
        /// Gets or sets the longitude of the location around which to retrieve place data.
        /// </summary>
        public double Longitude
        {
            get { return (double)GetValue(LongitudeProperty); }
            set { this.SetValue(LongitudeProperty, value); }
        }

        /// <summary>
        /// Identifies the Longitude dependency property.
        /// </summary>
        public static readonly DependencyProperty LongitudeProperty =
            DependencyProperty.Register("Longitude", typeof(double), typeof(PlacePicker), new PropertyMetadata(DefaultLongitude, OnLongitudePropertyChanged));

        private static async void OnLongitudePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            await placePicker.RefreshData();
        }
        
        #endregion Longitude

        #region TrackLocation

        /// <summary>
        /// Specifies whether to track the current location for searches.
        /// </summary>
        public bool TrackLocation
        {
            get { return (bool)GetValue(TrackLocationProperty); }
            set { this.SetValue(TrackLocationProperty, value); }
        }

        /// <summary>
        /// Identifies the TrackLocation dependency property.
        /// </summary>
        public static readonly DependencyProperty TrackLocationProperty =
            DependencyProperty.Register("TrackLocation", typeof(bool), typeof(PlacePicker), new PropertyMetadata(DefaultTrackLocation, OnTrackLocationPropertyChanged));

        private static async void OnTrackLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            if ((bool)e.NewValue)
            {
                placePicker.geoLocator = new Geolocator();
                placePicker.geoLocator.PositionChanged += placePicker.OnPositionChanged;
            }
            else if (placePicker.geoLocator != null)
            {
                placePicker.geoLocator.PositionChanged -= placePicker.OnPositionChanged;
                placePicker.geoLocator = null;
            }

            await placePicker.RefreshData();
        }

        #endregion TrackLocation

        #endregion Properties

        #region Implementation

        private async void OnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
#if NETFX_CORE
            await this.Dispatcher.RunAsync(
                CoreDispatcherPriority.Normal,
                async () =>
#endif
#if WINDOWS_PHONE
            this.Dispatcher.BeginInvoke(
                async () =>
#endif
            {
                await this.RefreshData();
            });
        }

        private async Task RefreshData()
        {
            this.Items.Clear();
            this.SelectedItems.Clear();

            if (!string.IsNullOrEmpty(this.AccessToken))
            {
                try
                {
                    var currentLocation = this.TrackLocation ? await this.GetCurrentLocation() : new LocationCoordinate(this.Latitude, this.Longitude);
                    FacebookClient facebookClient = new FacebookClient(this.AccessToken);

                    dynamic parameters = new ExpandoObject();
                    parameters.type = "place";
                    parameters.center = currentLocation.ToString();
                    parameters.distance = this.RadiusInMeters;
                    parameters.fields = this.DisplayFields;
                    if (!string.IsNullOrWhiteSpace(this.SearchText))
                    {
                        parameters.q = this.SearchText;
                    }

                    dynamic placesTaskResult = await facebookClient.GetTaskAsync("/search", parameters);
                    var data = (IEnumerable<dynamic>)placesTaskResult.data;
                    foreach (var item in data)
                    {
                        var place = new GraphPlace(item);
                        if (this.PlaceRetrieved.RaiseEvent(this, new DataItemRetrievedEventArgs<GraphPlace>(place), e => e.Exclude))
                        {
                            this.Items.Add(place);
                        }
                    }
                }
                catch (Exception ex)
                {
                    // TODO: review the types of exception that can be caught here
                    this.LoadFailed.RaiseEvent(this, new LoadFailedEventArgs("Error loading place data.", ex.Message));
                }
            }

            this.SetDataSource(this.Items);
            this.LoadCompleted.RaiseEvent(this, new DataReadyEventArgs<GraphPlace>(this.Items.ToList()));
        }

        private async Task<LocationCoordinate> GetCurrentLocation()
        {
            try
            {
                var position = await this.geoLocator.GetGeopositionAsync(new TimeSpan(0, 1, 0), new TimeSpan(0, 0, 0, 10));
                return new LocationCoordinate(position.Coordinate.Latitude, position.Coordinate.Longitude);
            }
            catch (System.UnauthorizedAccessException)
            {
                this.LoadFailed.RaiseEvent(this, new LoadFailedEventArgs("Error retrieving current location.", "Location is disabled."));
            }
            catch (TaskCanceledException)
            {
                this.LoadFailed.RaiseEvent(this, new LoadFailedEventArgs("Error retrieving current location.", "Task was cancelled."));
            }
            catch (Exception ex)
            {
                this.LoadFailed.RaiseEvent(this, new LoadFailedEventArgs("Error retrieving current location.", ex.Message));
            }

            // default location
            return new LocationCoordinate(DefaultLatitude, DefaultLongitude);
        }

        protected override IList GetData(IEnumerable<GraphPlace> items)
        {
            return items.Select(item => new PickerItem<GraphPlace>(this, item)).ToList();
        }

        protected override IEnumerable<GraphPlace> GetDesignTimeData()
        {
            return PlacePickerDesignSupport.DesignData;
        }

        #endregion Implementation
    }
}
