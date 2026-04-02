# EventHub

# Aplicație de Organizare a Evenimentelor cu Roluri: Organizator, Participant și Admin

## Scopul Aplicației
Aplicația de organizare a evenimentelor permite utilizatorilor să creeze, să gestioneze și să participe la evenimente. Utilizatorii pot fi organizatori, care creează evenimente, sau participanți, care pot confirma prezența și adăuga comentarii. De asemenea, există un rol de **Admin** care poate gestiona utilizatorii și poate șterge evenimente sau comentarii. 

Aplicația ajută utilizatorii să organizeze și să participe la evenimente, având o interfață simplă și funcțională. Aceasta include autentificare cu **email și parolă** sau **Google OAuth** și protejarea sesiunilor utilizatorilor cu **JWT**.

## Tehnologii Folosite

1. **Backend**:
   - **.NET Core (ASP.NET Core)**: Framework-ul principal pentru construirea API-ului RESTful.
   - **JWT (JSON Web Tokens)**: Folosit pentru autentificarea utilizatorilor și protejarea rutele API.
   - **OAuth 2.0**: Folosit pentru autentificarea cu **Google**.
   
2. **Frontend**:
   - **Angular**: Framework pentru frontend-ul aplicației.
   - **Angular Material**: Pentru a crea o interfață modernă și ușor de utilizat.

3. **Baza de Date**:
   - **SQL Server**: Pentru gestionarea utilizatorilor, evenimentelor, invitațiilor și comentariilor.

## Roluri Utilizatori

1. **Admin**:
   - Poate vizualiza toate evenimentele și poate șterge orice eveniment sau comentariu.
   - Poate gestiona utilizatorii (modificarea rolurilor, ștergerea conturilor).
   
2. **Organizator**:
   - Poate crea și edita evenimente.
   - Poate trimite invitații și vizualiza participanții.
   
3. **Participant**:
   - Poate vizualiza evenimentele la care a fost invitat.
   - Poate confirma prezența și poate adăuga comentarii la evenimentele la care participă.

## Tabele în Baza de Date

### 1. **Users**
| Câmp        | Tip       | Descriere                        |
|-------------|-----------|----------------------------------|
| Id          | int       | Cheie primară, identificator unic |
| Email       | string    | Adresa de email a utilizatorului |
| FullName    | string    | Numele complet al utilizatorului |
| PasswordHash| string    | Parola criptată a utilizatorului |
| Role        | string    | Rolul utilizatorului (Admin, Organizator, Participant) |
| CreatedAt   | DateTime  | Data creării contului            |
| IsActive    | bool      | Starea contului (activ sau inactiv) |

### 2. **Categories**
| Câmp        | Tip       | Descriere                        |
|-------------|-----------|----------------------------------|
| Id          | int       | Cheie primară, identificator unic |
| Name        | string    | Numele categoriei                |
| Description | string    | Descrierea categoriei            |

### 3. **Events**
| Câmp        | Tip       | Descriere                        |
|-------------|-----------|----------------------------------|
| Id          | int       | Cheie primară, identificator unic |
| Name        | string    | Numele evenimentului             |
| Date        | DateTime  | Data evenimentului               |
| Location    | string    | Locația evenimentului            |
| CategoryId  | int       | Cheie externă către Categories  |
| OrganizerId | int       | Cheie externă către Users       |
| CreatedAt   | DateTime  | Data creării evenimentului       |

### 4. **UserCategories**
| Câmp        | Tip       | Descriere                        |
|-------------|-----------|----------------------------------|
| UserId      | int       | Cheie externă către Users       |
| CategoryId  | int       | Cheie externă către Categories  |

### 5. **Invitations**
| Câmp        | Tip       | Descriere                        |
|-------------|-----------|----------------------------------|
| Id          | int       | Cheie primară, identificator unic |
| EventId     | int       | Cheie externă către Events      |
| UserId      | int       | Cheie externă către Users       |
| Status      | string    | Statusul invitației (Ex: Acceptat, Refuzat) |
| Message     | string    | Mesaj personalizat al invitației |
| SentAt      | DateTime  | Data trimiterii invitației      |

### 6. **EventRequests**
| Câmp        | Tip       | Descriere                        |
|-------------|-----------|----------------------------------|
| Id          | int       | Cheie primară, identificator unic |
| EventId     | int       | Cheie externă către Events      |
| UserId      | int       | Cheie externă către Users       |
| Status      | string    | Statusul cererii (Ex: Aprobat, Respins) |
| RequestedAt | DateTime  | Data cererii                    |
| RespondedAt | DateTime  | Data răspunsului                |

### 7. **Comments**
| Câmp        | Tip       | Descriere                        |
|-------------|-----------|----------------------------------|
| Id          | int       | Cheie primară, identificator unic |
| EventId     | int       | Cheie externă către Events      |
| UserId      | int       | Cheie externă către Users       |
| Content     | string    | Conținutul comentariului        |
| CreatedAt   | DateTime  | Data creării comentariului      |

## Relațiile între tabele

- **Users ↔ Events**: Un utilizator poate organiza mai multe evenimente (1:N).
- **Events ↔ Categories**: Fiecare eveniment aparține unei singure categorii (N:1).
- **Users ↔ Categories**: Utilizatorii pot fi interesați de mai multe categorii (M:N) — se realizează prin tabela intermediară `UserCategories`.
- **Users ↔ Invitations**: Utilizatorii pot primi invitații la mai multe evenimente (1:N).
- **Users ↔ EventRequests**: Utilizatorii pot face cereri pentru a participa la evenimente (1:N).
- **Events ↔ Comments**: Fiecare eveniment poate avea mai multe comentarii (1:N).
## Fluxul Aplicației

1. **Autentificare**:
   - Utilizatorii se autentifică cu **email și parolă**. După autentificare, backend-ul va genera un **token JWT** care va fi utilizat pentru a proteja rutele sensibile (de exemplu, crearea și vizualizarea evenimentelor).

2. **Crearea Evenimentului**:
   - **Organizatorii** pot crea un eveniment prin completarea unui formular cu detalii precum titlul, descrierea, locația și data evenimentului.

3. **Invitarea Participanților**:
   - **Organizatorii** pot trimite invitații participanților. Invitațiile vor fi trimise prin email, iar participanții vor putea confirma sau refuza invitația.

4. **Confirmarea Participării**:
   - **Participanții** pot vizualiza detalii despre evenimentele la care au fost invitați și pot confirma prezența lor.

5. **Moderarea Evenimentelor și Comentariilor**:
   - **Adminii** au privilegii complete asupra aplicației. Aceștia pot șterge sau edita orice eveniment și comentarii. De asemenea, pot gestiona utilizatorii, inclusiv schimbarea rolurilor sau ștergerea acestora.
