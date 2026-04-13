export interface Invitation {
  id: number;
  status: string;
  sentAt: string;
  respondedAt?: string;
  event: {
    id: number;
    title: string;
    description: string;
    location: string;
    eventDate: string;
    category: { id: number; name: string };
    organizer: { id: number; fullName: string };
  };
}