export interface Event {
  id: number;
  title: string;
  description: string;
  location: string;
  eventDate: string;
  category: { id: number; name: string };
  organizer: { id: number; fullName: string };
  participantCount?: number;
}