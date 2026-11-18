/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Ghi nhật ký ra Console; không dùng cơ sở dữ liệu
    public sealed class AuditLogObserver : IAlertObserver
    {
        public void Update(AlertEvent alertEvent)
        {
            Console.WriteLine(
                $"[AUDIT] {alertEvent.OccurredAt:HH:mm:ss} | " +
                $"{alertEvent.SensorName} | {alertEvent.Severity} | " +
                $"{alertEvent.Value:F1}{alertEvent.Unit}");
        }
    }
}