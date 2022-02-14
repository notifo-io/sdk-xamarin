# Handling notifications while a device is offline or airplane mode

For Android the solution is pretty easy, Notifo is able to send firebase [data](https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages) messages, which are non-collapsible by default and have a [default time-to-live period](https://firebase.google.com/docs/reference/fcm/rest/v1/projects.messages/#androidconfig) of four weeks, so we can receive all offline messages. But for iOS Apple will only store the most recent notification sent to a device on the [APNs servers](https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/APNSOverview.html#//apple_ref/doc/uid/TP40008194-CH8-SW5). So turns out for iOS we need a completely different custom solution.

First, we need to send different types of messages for Android and iOS. Android will need data messages and iOS will need notification messages. It is because iOS will treat data messages as [silent](https://developer.apple.com/documentation/usernotifications/setting_up_a_remote_notification_server/pushing_background_updates_to_your_app) messages and Apple [does not guarantee](https://firebase.google.com/docs/cloud-messaging/ios/receive#handle_silent_push_notifications) the delivery of silent messages and put other restrictions on them. 

In order to send platform-specific message payload, we need to use [Firebase HTTP v1 API](https://firebase.google.com/docs/cloud-messaging/migrate-v1). Then we can use a message format like in the example below.

```json
{
    "message": {
        "token": "<DEVICE TOKEN>",
        "android": {
            "data": {
                "title": "Title Android",
                "body": "Body Android"
            }
        },
        "apns": {
            "payload": {
                "aps": {
                    "alert": {
                        "title": "Title iOS",
                        "body": "Body iOS"
                    },
                    "mutable-content": 1
                }
            }
        }
    }
}
```

Second, when the last notification gets delivered after the device will back online we will send a delivery report to Notifo backend using a unique `trackingUrl`, then the Notifo backend will schedule a background/silent notification to wakeup the iOS device (no more than once per 30 minutes). This background notification will initiate fetch and show pending offline notifications as local notifications on the iOS device.

### Notification Service Extension
Apple doesn’t provide any official method to know when a Push Notification has been delivered to the user’s device. We are using [UNNotificationServiceExtension](https://developer.apple.com/documentation/usernotifications/unnotificationserviceextension) for that purposes. It turns out to be a viable solution, the only limitation is we have 30 seconds to send the delivery report to Notifo backend, but that feels more than enough. For this solution, there is included `mutable-content` parameter in the payload example above.

### App groups
For tracking/storing delivered notifications locally we are using `Xamarin.Essentials.Preferences` (that uses iOS `UserDefaults` on the iOS platform). In order to be able to share data between a host app and its app extension, we need to configure an [app group capability](https://developer.apple.com/documentation/bundleresources/entitlements/com_apple_security_application-groups).
Name convetion for app group is `group.{app-bundle-id}.notifo`.


