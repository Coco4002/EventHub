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

1. **Users (Utilizatori)**:

2. **Events (Evenimente)**:

3. **Invitations (Invitații)**:

4. **Comments (Comentarii)**:

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
