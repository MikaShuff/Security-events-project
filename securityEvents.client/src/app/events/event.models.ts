
// src/app/events/event.models.ts

// אירוע מינימלי שנציג בטבלה ובסינון

export interface Event {
  eventId: number;
  eventDate?: string | null;

  branchNum?: number | null;   // סניף (לפי swagger)
  eventType?: number | null;   // סוג אירוע
  subEventId?: number | null;  // תת סוג
  handleType?: number | null;  // אופן טיפול
  statusId?: number | null;    // סטטוס

  eventDesc?: string | null;   // תיאור אירוע
  eventSum?: number | null;    // סכום

  officerId?: number | null;
}


// פרמטרי סינון שנשלח לשרת
export interface EventQuery {
  fromDate?: string;   // 'YYYY-MM-DD'
  toDate?: string;     // 'YYYY-MM-DD'
  formatId?: number;
  branchId?: number;
  zoneId?: number;
  officerId?: number;
  eventTypeId?: number;
  subEventTypeId?: number;
  handlingId?: number;
  statusId?: number;

  // אופציונלי: עימוד/מיון בהמשך
  page?: number;
  pageSize?: number;
  sort?: string; // למשל: 'eventDate_desc'
}

// טיפוס בסיסי ללוקאפים
export interface LookupItem {
  id: number;
  name: string;
}
