/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;
using System.Collections.Generic;

namespace CampusSafetyHub
{
    // Subject: lưu observer, xác định ngưỡng rồi thông báo
    public sealed class SensorMonitor
    {
        private readonly List<IAlertObserver> observers = new();

        public void Attach(IAlertObserver observer)
        {
            ArgumentNullException.ThrowIfNull(observer);

            if (!observers.Contains(observer))
            {
                observers.Add(observer);
            }
        }

        public void Detach(IAlertObserver observer)
        {
            observers.Remove(observer);
        }

        public void CheckSensor(string sensorName, ISensor sensor)
        {
            ArgumentNullException.ThrowIfNull(sensor);

            double value = sensor.ReadValue();
            string? severity = value switch
            {
                >= 70 => "CRITICAL",
                >= 50 => "WARNING",
                _ => null
            };

            if (severity is null)
            {
                return;
            }

            var alertEvent = new AlertEvent(
                sensorName,
                value,
                sensor.GetUnit(),
                severity,
                DateTime.Now);

            Notify(alertEvent);
        }

        private void Notify(AlertEvent alertEvent)
        {
            // ToArray() giúp hủy đăng ký ngay khi đang thông báo vẫn an toàn
            foreach (IAlertObserver observer in observers.ToArray())
            {
                try
                {
                    observer.Update(alertEvent);
                }
                catch (Exception ex)
                {
                    // Một observer bị lỗi không ngăn các observer khác nhận sự kiện
                    Console.WriteLine($"[OBSERVER ERROR] {ex.Message}");
                }
            }
        }
    }
}