/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Observer mô phỏng bảng điều khiển
    public sealed class DashboardObserver : IAlertObserver
    {
        public void Update(AlertEvent alertEvent)
        {
            Console.WriteLine(
                $"[DASHBOARD] {alertEvent.SensorName}: " +
                $"{alertEvent.Value:F1}{alertEvent.Unit} " +
                $"[{alertEvent.Severity}]");
        }
    }
}