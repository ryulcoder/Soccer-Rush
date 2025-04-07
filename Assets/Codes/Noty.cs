using Unity.Notifications.Android;
using UnityEngine;
using UnityEngine.Android;

public class Noty : MonoBehaviour
{
    void Start()
    {
        if (Permission.HasUserAuthorizedPermission("android.permission.POST_NOTIFICATIONS") == false)
        {
            Permission.RequestUserPermission("android.permission.POST_NOTIFICATIONS");
        }
    }

    public void Show()
    {

        // 채널 등록
        var c = new AndroidNotificationChannel()
        {

            Id = "channel_id",
            Name = "Default Channel",
            Importance = Importance.Default,
            Description = "Generic notifications",

        };

        AndroidNotificationCenter.RegisterNotificationChannel(c);


        // 알림 생성
        var notification = new AndroidNotification();

        notification.Title = "Test";
        notification.Text = "This is a test for android notification.";
        notification.FireTime = System.DateTime.Now.AddSeconds(10);

        notification.SmallIcon = "icon_0";
        notification.LargeIcon = "icon_1";

        AndroidNotificationCenter.SendNotification(notification, "channel_id");

    }

}