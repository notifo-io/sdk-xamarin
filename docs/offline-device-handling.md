# Handling notifications while a device is offline or airplane mode

### The problem

Currently, Notifo supports only firebase [notification](https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages) messages that are [always collapsible](https://firebase.google.com/docs/cloud-messaging/concept-options#collapsible_and_non-collapsible_messages) and if we send multiple notifications to a device while it is offline, only the most recent notification will show when the device comes back online.

### Possible solution

For Android the solution is pretty easy, we just need to extend Notifo to be able to send firebase [data](https://firebase.google.com/docs/cloud-messaging/concept-options#notifications_and_data_messages) messages, which are non-collapsible by default and have a [default time-to-live period](https://firebase.google.com/docs/reference/fcm/rest/v1/projects.messages/#androidconfig) of four weeks (probably we need to reduce this), so we can receive all offline messages. But for iOS Apple will only store the most recent notification sent to a device on the [APNs servers](https://developer.apple.com/library/archive/documentation/NetworkingInternet/Conceptual/RemoteNotificationsPG/APNSOverview.html#//apple_ref/doc/uid/TP40008194-CH8-SW5). So turns out for iOS we need a completely different custom solution.

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
                },
                "notification-id": "unique-id"
            }
        }
    }
}
```

Second, when the last notification gets delivered after the device will back online we will send a delivery report to Notifo backend of that notification with a unique notification ID, then the backend will check for all the notifications that were not delivered and will send them again (that should be done only for iOS devices, so seems that we need to somehow distinguish between platforms, maybe send platform name together with device token).

Unfortunately, Apple doesn’t provide any official method to know when a Push Notification has delivered to the user’s device. But seems like [UNNotificationServiceExtension](https://developer.apple.com/documentation/usernotifications/unnotificationserviceextension) is a viable solution, the only limitation is we have 30 seconds to send the delivery report to Notifo backend, but that feels more than enough. For this solution, there are included “mutable-content” and “notification-id” parameters in the payload example above.


