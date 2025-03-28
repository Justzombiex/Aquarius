export interface Alert {
    id: string; // UUID
    message: string; // Alert description
    timeStamp: string; // ISO 8601 string format
    pondId: string; // UUID
    isActive: boolean //Alert active
    alarmType: string // [LowTemperature,HighTemperature,LowLevel,Disconnection]
  }