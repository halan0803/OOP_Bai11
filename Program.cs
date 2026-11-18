/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;
using System.IO;

namespace CampusSafetyHub
{
    public class Program
    {
        private static int passed;
        private static int failed;

        public static void Main()
        {
            Console.WriteLine("===== CAMPUS SAFETY HUB =====");

            // Adapter
            var legacy = new LegacySmokeDetector(400);
            ISensor sensor = new SmokeSensorAdapter(legacy);

            // Mediator
            var alarm = new Alarm();
            var door = new DoorController();
            var ventilation = new VentilationController();

            IEmergencyMediator mediator =
                new EmergencyMediator(alarm, door, ventilation);

            // Observer
            var monitor = new SensorMonitor();
            var dashboard = new DashboardObserver();
            var email = new EmailAlertObserver();

            monitor.Attach(dashboard);
            monitor.Attach(email);
            monitor.Attach(new AuditLogObserver());
            monitor.Attach(new EmergencyBridgeObserver(mediator));

            // CASE 1
            Console.WriteLine(
                "\n[CASE 1] 40%: binh thuong, khong canh bao");

            monitor.CheckSensor("Smoke-01", sensor);

            // CASE 2
            Console.WriteLine("\n[CASE 2] 55%: WARNING");

            legacy.SetSmokeLevel(550);
            monitor.CheckSensor("Smoke-01", sensor);

            // Minh họa Detach
            Console.WriteLine(
                "\nHuy dang ky EMAIL de minh hoa Observer.Detach()");

            monitor.Detach(email);

            // CASE 3
            Console.WriteLine("\n[CASE 3] 73.5%: CRITICAL");

            legacy.SetSmokeLevel(735);
            monitor.CheckSensor("Smoke-01", sensor);

            // Kiểm thử
            Console.WriteLine(
                "\n===== KIEM THU TU DONG =====");

            int failedTests = RunAll();

            Environment.ExitCode = failedTests == 0 ? 0 : 1;
        }

        // Observer hỗ trợ đếm số lần nhận cảnh báo
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

        // Observer giả lập lỗi
        private sealed class FailingObserver : IAlertObserver
        {
            public void Update(AlertEvent alertEvent)
            {
                throw new InvalidOperationException(
                    "Loi gia lap tu observer");
            }
        }

        // Đánh giá kết quả PASS/FAIL
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

        // Kiểm thử ngưỡng với đối tượng độc lập
        private static bool HasSeverity(
            int rawLevel, string? expectedSeverity)
        {
            ISensor sensor = new SmokeSensorAdapter(
                new LegacySmokeDetector(rawLevel));

            var monitor = new SensorMonitor();
            var observer = new CountingObserver();

            monitor.Attach(observer);
            monitor.CheckSensor("S1", sensor);

            if (expectedSeverity is null)
            {
                return observer.Count == 0;
            }

            return observer.Count == 1 &&
                observer.LastEvent?.Severity == expectedSeverity;
        }

        public static int RunAll()
        {
            passed = 0;
            failed = 0;

            // ===== NHÓM 1: ADAPTER =====

            Check("Adapter 735 -> 73.5",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(735)
                ).ReadValue() == 73.5);

            Check("Adapter tra don vi % smoke",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(0)
                ).GetUnit() == "% smoke");

            var legacyChanging = new LegacySmokeDetector(735);
            ISensor changingSensor =
                new SmokeSensorAdapter(legacyChanging);

            legacyChanging.SetSmokeLevel(700);

            Check("Adapter doc gia tri moi, khong sao chep",
                changingSensor.ReadValue() == 70.0);

            var legacyUpper = new LegacySmokeDetector(100);
            legacyUpper.SetSmokeLevel(1200);

            Check("Legacy clamp gia tri tren ve 1000",
                legacyUpper.GetSmokeLevel() == 1000);

            var legacyLower = new LegacySmokeDetector(100);
            legacyLower.SetSmokeLevel(-5);

            Check("Legacy clamp gia tri duoi ve 0",
                legacyLower.GetSmokeLevel() == 0);

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

            Check("Adapter clamp gia tri tren 100%",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(1200)
                ).ReadValue() == 100.0);

            Check("Adapter clamp gia tri duoi 0%",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(-20)
                ).ReadValue() == 0.0);

            // ===== NHÓM 2: NGƯỠNG CẢNH BÁO =====

            Check("49.9% khong canh bao",
                HasSeverity(499, null));

            Check("50.0% la WARNING",
                HasSeverity(500, AlertSeverity.Warning));

            Check("69.9% van la WARNING",
                HasSeverity(699, AlertSeverity.Warning));

            Check("70.0% la CRITICAL",
                HasSeverity(700, AlertSeverity.Critical));

            Check("Hang AlertSeverity dung chuoi theo de",
                AlertSeverity.Warning == "WARNING" &&
                AlertSeverity.Critical == "CRITICAL");

            // ===== NHÓM 3: OBSERVER =====

            // Kiểm tra đăng ký trùng
            var duplicateMonitor = new SensorMonitor();
            var duplicateObserver = new CountingObserver();

            duplicateMonitor.Attach(duplicateObserver);
            duplicateMonitor.Attach(duplicateObserver);

            duplicateMonitor.CheckSensor(
                "S2",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(500)));

            Check("Attach khong dang ky trung",
                duplicateObserver.Count == 1);

            // Kiểm tra hủy đăng ký
            var detachMonitor = new SensorMonitor();
            var detached = new CountingObserver();

            detachMonitor.Attach(detached);
            detachMonitor.Detach(detached);

            detachMonitor.CheckSensor(
                "S3",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(800)));

            Check("Detach ngung nhan thong bao",
                detached.Count == 0);

            // Kiểm tra ToArray khi Observer tự hủy
            var removingMonitor = new SensorMonitor();
            var leaving = new CountingObserver();
            var staying = new CountingObserver();

            leaving.OnUpdate =
                () => removingMonitor.Detach(leaving);

            removingMonitor.Attach(leaving);
            removingMonitor.Attach(staying);

            ISensor removingSensor = new SmokeSensorAdapter(
                new LegacySmokeDetector(800));

            removingMonitor.CheckSensor("S4", removingSensor);
            removingMonitor.CheckSensor("S4", removingSensor);

            Check("ToArray: observer tu huy dang ky an toan",
                leaving.Count == 1 && staying.Count == 2);

            // ===== NHÓM 4: LỖI OBSERVER =====

            var errorMonitor = new SensorMonitor();
            var healthy = new CountingObserver();

            errorMonitor.Attach(new FailingObserver());
            errorMonitor.Attach(healthy);

            string observerLog;
            TextWriter originalWriter = Console.Out;

            using (var writer = new StringWriter())
            {
                try
                {
                    Console.SetOut(writer);

                    errorMonitor.CheckSensor(
                        "S5",
                        new SmokeSensorAdapter(
                            new LegacySmokeDetector(700)));
                }
                finally
                {
                    Console.SetOut(originalWriter);
                }

                observerLog = writer.ToString();
            }

            Check("Loi observer khong chan observer khac",
                healthy.Count == 1);

            Check("Log loi ghi ten FailingObserver",
                observerLog.Contains(
                    "[OBSERVER ERROR] FailingObserver:"));

            // ===== NHÓM 5: MEDIATOR =====

            // WARNING không kích hoạt ứng phó
            var warningAlarm = new Alarm();
            var warningDoor = new DoorController();
            var warningVentilation = new VentilationController();

            var warningMonitor = new SensorMonitor();

            warningMonitor.Attach(
                new EmergencyBridgeObserver(
                    new EmergencyMediator(
                        warningAlarm,
                        warningDoor,
                        warningVentilation)));

            warningMonitor.CheckSensor(
                "S6",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(600)));

            Check("WARNING chua kich hoat emergency",
                !warningAlarm.IsActive &&
                !warningDoor.IsUnlocked &&
                !warningVentilation.IsStopped);

            // CRITICAL kích hoạt cả ba thiết bị
            var criticalAlarm = new Alarm();
            var criticalDoor = new DoorController();
            var criticalVentilation = new VentilationController();

            var criticalMonitor = new SensorMonitor();

            criticalMonitor.Attach(
                new EmergencyBridgeObserver(
                    new EmergencyMediator(
                        criticalAlarm,
                        criticalDoor,
                        criticalVentilation)));

            criticalMonitor.CheckSensor(
                "S7",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(700)));

            Check("CRITICAL kich hoat alarm, door, ventilation",
                criticalAlarm.IsActive &&
                criticalDoor.IsUnlocked &&
                criticalVentilation.IsStopped);

            // ===== NHÓM 6: CÔ LẬP LỖI THIẾT BỊ =====

            var failedAlarm = new Alarm();

            // Giả lập Door bị lỗi
            var failedDoor = new DoorController(
                simulateFailure: true);

            var continuedVentilation =
                new VentilationController();

            var failureMonitor = new SensorMonitor();

            failureMonitor.Attach(
                new EmergencyBridgeObserver(
                    new EmergencyMediator(
                        failedAlarm,
                        failedDoor,
                        continuedVentilation)));

            string deviceLog;

            originalWriter = Console.Out;

            using (var writer = new StringWriter())
            {
                try
                {
                    Console.SetOut(writer);

                    failureMonitor.CheckSensor(
                        "S8",
                        new SmokeSensorAdapter(
                            new LegacySmokeDetector(700)));
                }
                finally
                {
                    Console.SetOut(originalWriter);
                }

                deviceLog = writer.ToString();
            }

            Check(
                "Mediator: Door loi nhung Alarm va Ventilation van chay",
                failedAlarm.IsActive &&
                !failedDoor.IsUnlocked &&
                continuedVentilation.IsStopped);

            Check("Mediator: log loi chi ro DoorController",
                deviceLog.Contains(
                    "[DEVICE ERROR] DoorController:"));

            // ===== NHÓM 7: DỮ LIỆU CẢNH BÁO =====

            var dataMonitor = new SensorMonitor();
            var capture = new CountingObserver();

            dataMonitor.Attach(capture);

            dataMonitor.CheckSensor(
                "Smoke-01",
                new SmokeSensorAdapter(
                    new LegacySmokeDetector(735)));

            Check(
                "AlertEvent chua dung ten, gia tri, don vi, muc do",
                capture.LastEvent?.SensorName == "Smoke-01" &&
                capture.LastEvent.Value == 73.5 &&
                capture.LastEvent.Unit == "% smoke" &&
                capture.LastEvent.Severity == AlertSeverity.Critical);

            // Tổng kết
            Console.WriteLine(
                $"\nTONG KET: {passed} PASS, {failed} FAIL");

            return failed;
        }
    }
}