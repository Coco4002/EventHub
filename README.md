# EventHub

## Aplicație de Organizare a Evenimentelor cu Roluri: Organizator, Participant și Admin

---

## Scopul Aplicației

EventHub permite utilizatorilor să creeze, să gestioneze și să participe la evenimente.
Utilizatorii își pot seta categorii preferate, iar când un Organizator creează un eveniment
într-o categorie, invitațiile sunt trimise **automat** tuturor utilizatorilor interesați de acea categorie.
Participanții pot accepta sau refuza invitațiile și pot comenta la evenimentele la care participă.

---

## Tehnologii Folosite

### Backend
- **.NET 10 (ASP.NET Core)** — framework principal pentru API RESTful
- **Entity Framework Core 10** — ORM pentru comunicarea cu baza de date
- **SQL Server** — baza de date relațională
- **JWT (JSON Web Tokens)** — autentificare și protejarea rutelor
- **BCrypt.Net** — hashing parole
- **Scalar** — interfață vizuală pentru testarea API-ului

### Frontend *(în dezvoltare)*
- **Angular** — framework pentru interfața utilizatorului
- **Angular Material** — componente UI

---

## Roluri Utilizatori

### Admin
- Vizualizează toți utilizatorii și statistici generale
- Schimbă rolurile utilizatorilor
- Activează / dezactivează conturi
- Șterge orice eveniment, comentariu sau utilizator
- Creează categorii noi

### Organizator
- Creează, editează și șterge propriile evenimente
- Trimite invitații manual către participanți
- La crearea unui eveniment, invitațiile sunt trimise **automat** utilizatorilor abonați la categoria respectivă
- Vizualizează lista participanților și statusul invitațiilor
- Poate comenta la propriile evenimente

### Participant
- Vizualizează toate evenimentele și le filtrează după categorie
- Își setează categoriile preferate (subscribe / unsubscribe)
- Primește invitații automate la evenimentele din categoriile preferate
- Acceptă sau refuză invitațiile primite
- Poate comenta **doar** la evenimentele unde are invitația acceptată

---

## Structura Bazei de Date

### Users
| Câmp | Tip | Descriere |
|---|---|---|
| Id | int | Cheie primară |
| Email | string | Email unic |
| FullName | string | Numele complet |
| PasswordHash | string | Parola hashed cu BCrypt |
| Role | string | Admin / Organizator / Participant |
| CreatedAt | DateTime | Data creării contului |
| IsActive | bool | Cont activ sau dezactivat |

### Categories
| Câmp | Tip | Descriere |
|---|---|---|
| Id | int | Cheie primară |
| Name | string | Numele categoriei |
| Description | string | Descrierea categoriei |
| CreatedAt | DateTime | Data creării |

### Events
| Câmp | Tip | Descriere |
|---|---|---|
| Id | int | Cheie primară |
| Title | string | Titlul evenimentului |
| Description | string | Descrierea evenimentului |
| Location | string | Locația |
| EventDate | DateTime | Data evenimentului |
| CategoryId | int | FK → Categories |
| OrganizerId | int | FK → Users |
| CreatedAt | DateTime | Data creării |

### UserCategories *(tabel de legătură M:N)*
| Câmp | Tip | Descriere |
|---|---|---|
| Id | int | Cheie primară |
| UserId | int | FK → Users |
| CategoryId | int | FK → Categories |
| SubscribedAt | DateTime | Data abonării |

### Invitations
| Câmp | Tip | Descriere |
|---|---|---|
| Id | int | Cheie primară |
| EventId | int | FK → Events |
| ParticipantId | int | FK → Users |
| Status | string | Pending / Accepted / Declined |
| SentAt | DateTime | Data trimiterii |
| RespondedAt | DateTime? | Data răspunsului |

### EventRequests
| Câmp | Tip | Descriere |
|---|---|---|
| Id | int | Cheie primară |
| EventId | int | FK → Events |
| UserId | int | FK → Users |
| Status | string | Statusul cererii |
| Message | string | Mesaj opțional |
| RequestedAt | DateTime | Data cererii |
| RespondedAt | DateTime? | Data răspunsului |

### Comments
| Câmp | Tip | Descriere |
|---|---|---|
| Id | int | Cheie primară |
| EventId | int | FK → Events |
| UserId | int | FK → Users |
| Content | string | Conținutul comentariului |
| CreatedAt | DateTime | Data creării |
| UpdatedAt | DateTime? | Data ultimei editări |

---

## Relații între Tabele

- **Users ↔ Events** — un Organizator poate crea multe evenimente (1:N)
- **Events ↔ Categories** — fiecare eveniment aparține unei singure categorii (N:1)
- **Users ↔ Categories** — utilizatorii pot prefera mai multe categorii (M:N) prin `UserCategories`
- **Events ↔ Invitations** — un eveniment poate avea multe invitații (1:N)
- **Events ↔ Comments** — un eveniment poate avea multe comentarii (1:N)
- **Events ↔ EventRequests** — un eveniment poate primi mai multe cereri de participare (1:N)

---

## Endpoint-uri API

### Auth
| Metodă | Endpoint | Acces | Descriere |
|---|---|---|---|
| POST | /api/auth/register | Public | Înregistrare cont nou |
| POST | /api/auth/login | Public | Autentificare, returnează JWT |

### Events
| Metodă | Endpoint | Acces | Descriere |
|---|---|---|---|
| GET | /api/events | Toți | Listează evenimente, filtru opțional `?categoryId=X` |
| GET | /api/events/{id} | Toți | Detalii eveniment |
| GET | /api/events/my | Organizator, Admin | Evenimentele mele |
| POST | /api/events | Organizator, Admin | Creează eveniment + invitații automate |
| PUT | /api/events/{id} | Organizatorul lui, Admin | Editează eveniment |
| DELETE | /api/events/{id} | Organizatorul lui, Admin | Șterge eveniment |

### Invitations
| Metodă | Endpoint | Acces | Descriere |
|---|---|---|---|
| GET | /api/invitations/my | Toți | Invitațiile mele |
| GET | /api/invitations/event/{id} | Organizator, Admin | Invitațiile unui eveniment |
| POST | /api/invitations | Organizator, Admin | Trimite invitație manuală |
| PUT | /api/invitations/{id}/respond | Participant | Acceptă sau refuză invitația |
| DELETE | /api/invitations/{id} | Organizator, Admin | Anulează invitație |

### Comments
| Metodă | Endpoint | Acces | Descriere |
|---|---|---|---|
| GET | /api/comments/event/{id} | Toți | Comentariile unui eveniment |
| POST | /api/comments | Invitat acceptat, Organizator, Admin | Adaugă comentariu |
| PUT | /api/comments/{id} | Autorul comentariului | Editează comentariu |
| DELETE | /api/comments/{id} | Autorul, Organizatorul, Admin | Șterge comentariu |

### Categories
| Metodă | Endpoint | Acces | Descriere |
|---|---|---|---|
| GET | /api/categories | Toți | Listează toate categoriile |
| GET | /api/categories/my | Toți | Categoriile mele preferate |
| POST | /api/categories/{id}/subscribe | Toți | Abonare la categorie |
| DELETE | /api/categories/{id}/unsubscribe | Toți | Dezabonare de la categorie |
| POST | /api/categories | Admin | Creează categorie nouă |

### Admin
| Metodă | Endpoint | Acces | Descriere |
|---|---|---|---|
| GET | /api/admin/users | Admin | Toți utilizatorii |
| GET | /api/admin/stats | Admin | Statistici generale |
| PUT | /api/admin/users/{id}/role | Admin | Schimbă rolul unui user |
| PUT | /api/admin/users/{id}/deactivate | Admin | Dezactivează cont |
| PUT | /api/admin/users/{id}/activate | Admin | Activează cont |
| DELETE | /api/admin/users/{id} | Admin | Șterge cont |

---

## Fluxul Principal al Aplicației
```
1. Utilizatorul se înregistrează → POST /api/auth/register
2. Se autentifică → POST /api/auth/login → primește JWT token
3. Își setează categoriile preferate → POST /api/categories/{id}/subscribe
4. Organizatorul creează un eveniment → POST /api/events
   → invitații trimise automat la toți abonații categoriei
5. Participantul vede invitațiile → GET /api/invitations/my
6. Participantul acceptă → PUT /api/invitations/{id}/respond
7. Participantul comentează → POST /api/comments
8. Adminul monitorizează → GET /api/admin/stats
```

---

## Instalare și Rulare

### Cerințe
- .NET 10 SDK
- SQL Server (sau SQL Server Express)

### Pași
```bash
# Clonează repo-ul
git clone https://github.com/Coco4002/EventHub.git
cd EventHub

# Configurează connection string în appsettings.json
# "DefaultConnection": "Server=.\\SQLEXPRESS;Database=EventHubDb;Trusted_Connection=True;TrustServerCertificate=True;"

# Rulează migrațiile
dotnet ef database update

# Pornește aplicația
dotnet run
```

API-ul va fi disponibil la `http://localhost:5090`
Documentația interactivă la `http://localhost:5090/scalar/v1`
