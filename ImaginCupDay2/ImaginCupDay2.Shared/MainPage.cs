using System;
using System.Collections.Generic;
using System.Text;
using Windows.Devices.Geolocation;
using Windows.Devices.Geolocation.Geofencing;
using Windows.Services.Maps;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Notifications;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Maps;

namespace ImaginCupDay2
{
#if WINDOWS_PHONE_APP
    public sealed partial class MainPage
    {
        Geoposition geoposition;
        Geolocator geolocator;
        string getLat;
        string getLong;
        MapIcon currentPin;
        GeofenceMonitor myMonitor = GeofenceMonitor.Current;

        protected override async void OnNavigatedTo(Windows.UI.Xaml.Navigation.NavigationEventArgs e)
        {
            base.OnNavigatedTo(e);

            myMonitor.Geofences.Clear();

            geolocator = new Geolocator();
            geolocator.DesiredAccuracy = PositionAccuracy.High;
            geolocator.MovementThreshold = 1;
            geolocator.PositionChanged += geolocator_PositionChanged;

            BasicGeoposition bs = new BasicGeoposition()
            {
                Latitude = 13.7649,
                Longitude = 100.5383
            };

            currentPin = new MapIcon()
            {
                Title = "I am here"
            };

            MapIcon vm = new MapIcon()
            {
                Title = "Victory Monument",
                Location = new Geopoint(bs)
            };

            myMap.MapElements.Add(currentPin);
            myMap.MapElements.Add(vm);

            Geofence btsVM = new Geofence("btsvm", new Geocircle(new BasicGeoposition() { Latitude = 13.7621, Longitude = 100.5367 },50));
            Geofence payaThai = new Geofence("payathai", new Geocircle(new BasicGeoposition() { Latitude = 13.7583, Longitude = 100.5346 }, 50));

            try
            {
                myMonitor.Geofences.Add(btsVM);
                myMonitor.Geofences.Add(payaThai);
            }
            catch (Exception ex)
            {
                
            }

            myMonitor.GeofenceStateChanged += myMonitor_GeofenceStateChanged;
        }

        void myMonitor_GeofenceStateChanged(GeofenceMonitor sender, object args)
        {
            var getReport = sender.ReadReports();

            foreach (var report in getReport)
            {
                if (report.Geofence.Id == "btsvm")
                {
                    if (report.NewState == GeofenceState.Entered)
                    {
                        Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            //TextBlock tb = new TextBlock();
                            //tb.Text = "Welcome to BTS Victory Monument";
                            //reportField.Children.Add(tb);

                            var notifier = ToastNotificationManager.CreateToastNotifier();
                            notifier.Show(BuildToast("Welcome to BTS Victory Monument"));
                        });
                    }
                    else if (report.NewState == GeofenceState.Exited)
                    {
                         Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            TextBlock tb = new TextBlock();
                            tb.Text = "Exit from BTS Victory Monument";
                            reportField.Children.Add(tb);
                        });
                    }
                }
                else if (report.Geofence.Id == "payathai")
                {
                    if (report.NewState == GeofenceState.Entered)
                    {
                         Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            TextBlock tb = new TextBlock();
                            tb.Text = "Welcome to Payathai";
                            reportField.Children.Add(tb);
                        });
                    }
                    else if (report.NewState == GeofenceState.Exited)
                    {
                         Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
                        {
                            TextBlock tb = new TextBlock();
                            tb.Text = "Exit from Payathai";
                            reportField.Children.Add(tb);
                        });
                    }
                }
            }
        }

        private async void geolocator_PositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            geoposition = await geolocator.GetGeopositionAsync();

            Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                latTbl.Text = geoposition.Coordinate.Latitude.ToString();
                longTbl.Text = geoposition.Coordinate.Longitude.ToString();

                getLat = geoposition.Coordinate.Latitude.ToString();
                getLong = geoposition.Coordinate.Longitude.ToString();

                myMap.Center = geoposition.Coordinate.Point;
                currentPin.Location = geoposition.Coordinate.Point;
            });
        }

        private async void extMapBtn_Click(object sender, RoutedEventArgs e)
        {
            string uriTolaunch = @"bingmaps:?cp=" + getLat + "~" + getLong + "&lvl=17";
            var getUri = new Uri(uriTolaunch);
            var run = await Windows.System.Launcher.LaunchUriAsync(getUri);
        }

        private async void routeBtn_Click(object sender, RoutedEventArgs e)
        {
            BasicGeoposition startPoint = new BasicGeoposition()
            {
                Latitude = 13.7617,
                Longitude = 100.5365
            };
            BasicGeoposition endPoint = new BasicGeoposition()
            {
                Latitude = 13.7642,
                Longitude = 100.5401
            };
            MapRouteFinderResult routeFinder = await MapRouteFinder.GetDrivingRouteAsync(new Geopoint(startPoint), new Geopoint(endPoint),MapRouteOptimization.Distance,MapRouteRestrictions.None);
            if (routeFinder.Status == MapRouteFinderStatus.Success)
            {
                MapRouteView view = new MapRouteView(routeFinder.Route)
                {
                    RouteColor = Colors.Blue,
                    OutlineColor = Colors.Yellow
                };

                myMap.Routes.Add(view);
                await myMap.TrySetViewBoundsAsync(routeFinder.Route.BoundingBox,null,MapAnimationKind.None);
            }
        }
        public ToastNotification BuildToast(string content,string arg=null)
        {
            var template = ToastTemplateType.ToastText01;
            var getTemplate = ToastNotificationManager.GetTemplateContent(template);
            var getXML = getTemplate.GetElementsByTagName("text");
            var text = getTemplate.CreateTextNode(content);
            getXML[0].AppendChild(text);
            return new ToastNotification(getTemplate);
        }
    }
#endif
}
