/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    public class Program
    {
        public static void Main()
        {
            Console.WriteLine("===== CAMPUS SAFETY HUB =====");

            // Adapter: kết nối cảm biến cũ với giao diện chuẩn
            var legacy = new LegacySmokeDetector(400);
            ISensor sensor = new SmokeSensorAdapter(legacy);

            // Mediator: các thiết bị được điều phối từ một nơi
            var alarm = new Alarm();
            var door = new DoorController();
            var ventilation = new VentilationController();
            IEmergencyMediator mediator =
                new EmergencyMediator(alarm, door, ventilation);

            // Observer: mỗi loại kênh thông báo được đăng ký độc lập
            var monitor = new SensorMonitor();
            var dashboard = new DashboardObserver();
            var email = new EmailAlertObserver();
            monitor.Attach(dashboard);
            monitor.Attach(email);
            monitor.Attach(new AuditLogObserver());
            monitor.Attach(new EmergencyBridgeObserver(mediator));

            Console.WriteLine("\n[CASE 1] 40%: binh thuong, khong canh bao");
            monitor.CheckSensor("Smoke-01", sensor);

            Console.WriteLine("\n[CASE 2] 55%: WARNING");
            legacy.SetSmokeLevel(550);
            monitor.CheckSensor("Smoke-01", sensor);

            Console.WriteLine("\nHuy dang ky EMAIL de minh hoa Observer.Detach()");
            monitor.Detach(email);

            Console.WriteLine("\n[CASE 3] 73.5%: CRITICAL");
            legacy.SetSmokeLevel(735);
            monitor.CheckSensor("Smoke-01", sensor);

            Console.WriteLine("\n===== KIEM THU TU DONG =====");
            int failed = RunAll();
            Environment.ExitCode = failed == 0 ? 0 : 1;
        }

        private static int passed;
        private static int failed;

        private sealed class CountingObserver : IAlertObserver
        {
            public int Count { get; private set; }
            public AlertEvent? LastEvent { get; private set; }
            public Action? OnUpdate { get; set; }

            public void Update(AlertEvent alertEvent)
            {
                Count++;
                LastEvent = alertEvent;
                OnUpdate?.Invoke();
            }
        }

        private sealed class FailingObserver : IAlertObserver
        {
            public void Update(AlertEvent alertEvent)
            {
                throw new InvalidOperationException("Loi gia lap tu observer");
            }
        }

        private static void Check(string name, bool condition)
        {
            if (condition)
            {
                passed++;
                Console.WriteLine($"[PASS] {name}");
            }
            else
            {
                failed++;
                Console.WriteLine($"[FAIL] {name}");
            }
        }

        public static int RunAll()
        {
            passed = 0;
            failed = 0;

            // 1. Adapter: chuyển thang, đơn vị và đọc trạng thái mới
            var legacy = new LegacySmokeDetector(735);
            ISensor sensor = new SmokeSensorAdapter(legacy);
            Check("Adapter 735 -> 73.5", sensor.ReadValue() == 73.5);
            Check("Adapter tra don vi % smoke", sensor.GetUnit() == "% smoke");
            legacy.SetSmokeLevel(700);
            Check("Adapter doc gia tri moi, khong sao chep", sensor.ReadValue() == 70.0);
            legacy.SetSmokeLevel(1200);
            Check("Legacy clamp gia tri tren ve 1000", sensor.ReadValue() == 100.0);
            legacy.SetSmokeLevel(-5);
            Check("Legacy clamp gia tri duoi ve 0", sensor.ReadValue() == 0.0);

            bool nullRejected = false;
            try
            {
                _ = new SmokeSensorAdapter(null!);
            }
            catch (ArgumentNullException)
            {
                nullRejected = true;
            }
            Check("Adapter tu choi legacy null", nullRejected);

            // 2. Observer: ngưỡng, đăng ký trùng và hủy đăng ký
            var monitor = new SensorMonitor();
            var observer = new CountingObserver();
            monitor.Attach(observer);
            monitor.Attach(observer); // Không được nhân đôi thông báo

            legacy.SetSmokeLevel(499);
            monitor.CheckSensor("S1", sensor);
            Check("49.9% khong canh bao", observer.Count == 0);

            legacy.SetSmokeLevel(500);
            monitor.CheckSensor("S1", sensor);
            Check("50.0% la WARNING; khong dang ky trung",
                observer.Count == 1 && observer.LastEvent?.Severity == "WARNING");

            legacy.SetSmokeLevel(699);
            monitor.CheckSensor("S1", sensor);
            Check("69.9% van la WARNING",
                observer.Count == 2 && observer.LastEvent?.Severity == "WARNING");

            legacy.SetSmokeLevel(700);
            monitor.CheckSensor("S1", sensor);
            Check("70.0% la CRITICAL",
                observer.Count == 3 && observer.LastEvent?.Severity == "CRITICAL");

            monitor.Detach(observer);
            legacy.SetSmokeLevel(800);
            monitor.CheckSensor("S1", sensor);
            Check("Detach ngung nhan thong bao", observer.Count == 3);

            // 3. Observer có thể hủy đăng ký ngay trong khi đang Notify
            var monitor2 = new SensorMonitor();
            var leaving = new CountingObserver();
            var staying = new CountingObserver();
            leaving.OnUpdate = () => monitor2.Detach(leaving);
            monitor2.Attach(leaving);
            monitor2.Attach(staying);
            monitor2.CheckSensor("S2", sensor);
            monitor2.CheckSensor("S2", sensor);
            Check("ToArray: observer tu huy dang ky an toan",
                leaving.Count == 1 && staying.Count == 2);

            // 4. Một observer lỗi không ảnh hưởng observer khác
            var monitor3 = new SensorMonitor();
            var healthy = new CountingObserver();
            monitor3.Attach(new FailingObserver());
            monitor3.Attach(healthy);
            monitor3.CheckSensor("S3", sensor);
            Check("Loi observer khong chan observer khac", healthy.Count == 1);

            // 5. Mediator: WARNING chưa kích hoạt, CRITICAL kích hoạt cả 3
            var alarm = new Alarm();
            var door = new DoorController();
            var ventilation = new VentilationController();
            IEmergencyMediator mediator =
                new EmergencyMediator(alarm, door, ventilation);
            var monitor4 = new SensorMonitor();
            monitor4.Attach(new EmergencyBridgeObserver(mediator));

            legacy.SetSmokeLevel(600);
            monitor4.CheckSensor("S4", sensor);
            Check("WARNING chua kich hoat emergency",
                !alarm.IsActive && !door.IsUnlocked && !ventilation.IsStopped);

            legacy.SetSmokeLevel(700);
            monitor4.CheckSensor("S4", sensor);
            Check("CRITICAL kich hoat alarm, door, ventilation",
                alarm.IsActive && door.IsUnlocked && ventilation.IsStopped);

            // 6. Kiểm tra dữ liệu AlertEvent đến observer không bị biến đổi
            var monitor5 = new SensorMonitor();
            var capture = new CountingObserver();
            monitor5.Attach(capture);
            legacy.SetSmokeLevel(735);
            monitor5.CheckSensor("Smoke-01", sensor);
            Check("AlertEvent chua dung ten, gia tri, don vi, muc do",
                capture.LastEvent?.SensorName == "Smoke-01" &&
                capture.LastEvent.Value == 73.5 &&
                capture.LastEvent.Unit == "% smoke" &&
                capture.LastEvent.Severity == "CRITICAL");

            Console.WriteLine($"\nTONG KET: {passed} PASS, {failed} FAIL");
            return failed;
        }
    }
}