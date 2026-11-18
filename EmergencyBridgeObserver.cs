/********************************
Mã sinh viên: 202418931
Họ tên: Trần Thị Hà Lan
********************************/

using System;

namespace CampusSafetyHub
{
    // Observer cầu nối: chỉ gửi CRITICAL sang Mediator
    public sealed class EmergencyBridgeObserver : IAlertObserver
    {
        private readonly IEmergencyMediator mediator;

        public EmergencyBridgeObserver(IEmergencyMediator mediator)
        {
            this.mediator = mediator ??
                throw new ArgumentNullException(nameof(mediator));
        }

        public void Update(AlertEvent alertEvent)
        {
            if (alertEvent.Severity == AlertSeverity.Critical)
            {
                mediator.HandleEmergency(alertEvent);
            }
        }
    }
}