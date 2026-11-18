/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Chỉ giả lập email trên Console, không gửi email thật
    public sealed class EmailAlertObserver : IAlertObserver
    {
        public void Update(AlertEvent alertEvent)
        {
            Console.WriteLine($"[EMAIL SIMULATION] {alertEvent.SensorName}: {alertEvent.Severity}");
        }
    }
}