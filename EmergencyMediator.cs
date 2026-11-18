/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Mediator điều phối 3 thiết bị, chúng không phụ thuộc trực tiếp nhau
    public sealed class EmergencyMediator : IEmergencyMediator
    {
        private readonly Alarm alarm;
        private readonly DoorController door;
        private readonly VentilationController ventilation;

        public EmergencyMediator(Alarm alarm, DoorController door,
                                 VentilationController ventilation)
        {
            this.alarm = alarm ?? throw new ArgumentNullException(nameof(alarm));
            this.door = door ?? throw new ArgumentNullException(nameof(door));
            this.ventilation = ventilation ??
                throw new ArgumentNullException(nameof(ventilation));
        }

        public void HandleEmergency(AlertEvent alertEvent)
        {
            if (alertEvent.Severity != "CRITICAL")
            {
                return;
            }

            Console.WriteLine("[MEDIATOR] Kich hoat ung pho CRITICAL.");
            // Xử lý Alarm
            try
            {
                alarm.Activate();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[DEVICE ERROR] Alarm: {ex.Message}");
            }

            // Xử lý Door
            try
            {
                door.Unlock();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[DEVICE ERROR] DoorController: {ex.Message}");
            }

            // Xử lý Ventilation
            try
            {
                ventilation.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine(
                    $"[DEVICE ERROR] VentilationController: {ex.Message}");
            }
        }
    }
}