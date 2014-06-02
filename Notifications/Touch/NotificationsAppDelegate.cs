using Cheesebaron.MvxPlugins.Notifications.Messages;
using Cirrious.CrossCore;
using Cirrious.MvvmCross.Plugins.Messenger;
using Cirrious.MvvmCross.Touch.Platform;
using MonoTouch.Foundation;
using MonoTouch.UIKit;

namespace Cheesebaron.MvxPlugins.Notifications
{
    public class NotificationsAppDelegate : MvxApplicationDelegate
    {
        public override bool FinishedLaunching(UIApplication application, NSDictionary options)
        {
            if(options != null) {
                if(options.ContainsKey(UIApplication.LaunchOptionsLocalNotificationKey)) {
                    // was woken from Local notification
                    var localNotification =
                        options[UIApplication.LaunchOptionsLocalNotificationKey] as
                            UILocalNotification;

                    if(localNotification != null) 
                        ReceivedLocalNotification(localNotification);
                }
                else if(options.ContainsKey(UIApplication.LaunchOptionsRemoteNotificationKey)) {
                    var remoteNotification =
                        options[UIApplication.LaunchOptionsRemoteNotificationKey] as NSDictionary;

                    if(remoteNotification != null)
                        ReceivedRemoteNotification(remoteNotification);
                }
            }

            return true;
        }

        private void ReceivedRemoteNotification(NSDictionary dict)
        {
            NSError error;
            var jsonData = NSJsonSerialization.Serialize(dict,
                NSJsonWritingOptions.PrettyPrinted, out error);

            if (error != null)
            {
                var json = new NSString(jsonData, NSStringEncoding.UTF8).ToString();

                Mvx.Resolve<MvxMessengerHub>().Publish(new NotificationReceivedMessage(this)
                {
                    Body = json,
                    Local = false
                });
            }
        }

        private void ReceivedLocalNotification(UILocalNotification notification)
        {
            Mvx.Resolve<MvxMessengerHub>().Publish(new NotificationReceivedMessage(this)
            {
                Body = notification.AlertBody,
                Local = true,
                AlertAction = notification.AlertAction
            });
        }

        public override void ReceivedRemoteNotification(
            UIApplication application, NSDictionary userInfo)
        {
            base.ReceivedRemoteNotification(application, userInfo);
            
            if (userInfo != null)
                ReceivedRemoteNotification(userInfo);
        }

        public override void ReceivedLocalNotification(UIApplication application, 
            UILocalNotification notification)
        {
            base.ReceivedLocalNotification(application, notification);

            if (notification != null)
                ReceivedLocalNotification(notification);
        }
    }
}