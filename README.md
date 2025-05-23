# Hello, and welcome to the Remote Waitlist Manager project, Lequeuer!
A working demo of the project can be found at: https://dev.azalea.life/

# Table of Contents

- [Project Overview](#project-overview)
- [Backend Overview: REST API (.Net Core 9) + RTC](#backend-rest-api-net-core-9--rtc)
- [Frontend Overview: SPA (Angular, Angular Material)](#frontend-spa-angular-angular-material)
- [Considerations and Assumptions](#considerations-and-assumptions)
- [Example interaction flow - Reservation creation](#example-interaction-flow---reservation-creation)
- [Run project locally](#run-project-locally)


- SECTIONS AFTER THIS WERE COPIED FROM THE ASSIGNMENT
- [TableCheck SWE Fullstack Take-Home Assignment](#tablecheck-swe-fullstack-take-home-assignment)
- [Technical Requirements](#technical-requirements)
- [Business Requirements](#business-requirements)
- [Submission Guidelines](#submission-guidelines)
- [Evaluation Criteria](#evaluation-criteria)

# Project Overview

## Backend: REST API (.Net Core 9) + RTC

- The core components of the backend are the ReservationsService, ReservationsHub, and the RestaurantsService.
- The `RestaurantsService` is responsible for keeping track of the restaurant's available seats.
- The `ReservationsServices` contains the logic for managing reservations, including adding new reservations, checking
  in parties, and queueing.
- The `ReservationsHub` is a SignalR hub that allows real-time communication between the server and clients. It notifies
  clients when their reservation status changes, such as when it's their turn to check in, or when there are changes
  made by other clients. This allows for a seamless experience where users can see updates in real-time without needing
  to refresh the page.
- The backend uses a vertical slice architecture.
- Functionalities are grouped in the Modules folder. eg: Features related to reservations are in the
  Modules/ReservationModule folder.

## Frontend: SPA (Angular, Angular Material)

- The core components of the frontend are the ff:
    - `TablesStatusDisplay`: This component displays the status of the restaurant's tables, including the number of
      available seats and the current reservations' statuses
    - `QueuePositionDisplay`: This component displays the user's position in the queue. They can see how many parties
      are ahead of them and how many seats are available.
    - `CheckIn`: This component shows the current user's reservation status and allows them to check in when it's their
      turn.
    - `RtcService`: This service is responsible for managing the real-time communication with the backend using SignalR.
      It connects to the ReservationsHub and listens for updates on reservation statuses for the **current** restaurant
      only.
    - `ReservationService`: This service is responsible for managing the reservation process, including adding new
      reservations and checking in for the frontend.

## Considerations and Assumptions

- Project assumes that there would be a terminal or device that the restaurant staff would use to finish the service for a party.
  To simulate this, the `Dequeuer` service was created. It will finish the service for a party after [party size] * 3
  seconds.
- The app does handle cases where a party creates a reservation and then leaves the restaurant without checking
  in.
- The app assumes that the user will go to the physical restaurant and will be presented with a QR code or some other
  means that will lead them to the `/restaurants/{restaurantId}` page. This will take them to the restaurant's reservation page.

## Example interaction flow - Reservation creation
It might be easier to understand the resposibilities of the different components by looking at an example interaction. <br/>
The following diagram illustrates the flow of data between the different components when a user creates a reservation. <br/> 
Text explanation follows the diagram

![lequeuer-data-flow](https://github.com/user-attachments/assets/a54d110c-3f16-49a5-a45f-239c6df09c0f)
1. Client **`ReservationService`** sends POST request to **`ReservationEndpoint`**
2. Backend **`ReservationEndpoint`** passes the request to **`ReservationsService`**
   1. **`ReservationsService`** validates request data, creates reservation in DB
   2. **`ReservationsService`** calls _ReservationDataUpdated_ in the **`ReservationsHub`**
3. **`ReservationsService`** returns new reservation to Backend **`ReservationEndpoint`**
4. **`ReservationEndpoint`** returns 201 to Client **`ReservationService`**
5. **`ReservationsHub`** publishes _OnReservationDataUpdated_ in RTC channel
6. Client **`RTCService`** receives _OnReservationDataUpdated_ message and emits a _OnReservationDataUpdated_ message on the frontend
7. Client **`ReservationService`** gets the message and sends a GET request to **`ReservationsEndpoint`** for new data.
8. The service sets the new data into a signal.
9. The signal propagates to the interested components

# Run project locally

1. Clone the repository~~~~
2. Cd into the root directory of the project (where docker-compose.yaml is located)
3. Run `docker compose up` to start the application
    - This should do the ff automatically:
        - Build the frontend and backend image
        - Create the network
        - Create the volumes
        - Create the database
           - The database is mapped to port 3399 to avoid port conflicts in case the tester already has a mysql instance
        - Run the migrations
        - Start the backend and frontend services
4. Open your browser and navigate to `http://localhost:4200` to access the application

# TableCheck SWE Fullstack Take-Home Assignment

Remote Waitlist Manager is a full-stack application designed to handle the waitlist of your restaurant. It manages
seating, queueing, and notifications for your diners. **Multiple parties** should be able to join your restaurant's
waitlist **concurrently**. Instead of waiting in line to write your name on a piece of paper, you can now join the
waitlist virtually and get notified when your table is ready. This will increase your restaurant's efficiency and
provide a better experience for your customers.

The user flow is as follows:

- A party of diners go to their favorite restaurant. It's fully booked, but the restaurant gives the option to join a
  virtual waitlist accessible via browser.
- When the diner opens the app they're asked to input their name and party size.
- After joining the waitlist, they can check the app to verify if it's their turn.
- When the table is ready for them, they check in via the app and get seated.

## Technical Requirements

### Frontend

Our current tech stack uses ReactJS, TypeScript and isomorphic SSR, but you shouldn’t be limited to that. If you feel
more proficient with a different stack, just go for it! Feel free to use an SPA, islands, traditional SSR, vue, angular,
ember, vanilla JS, etc.

### Backend

Similarly, while our stack uses Ruby on Rails with MongoDB, you’re free to use any mainstream language/framework and
storage.

Whatever database you decide to use, it should be runnable with a simple `docker compose up`.

## Business Requirements

**Restaurant Capacity**

Hardcoded to 10 seats.

**Service Time Calculation**

Hardcoded to 3 seconds per person. Example: A party of 4 takes 12 seconds to complete the service.

**Joining the waitlist**

The diner opens the app that shows a single form with these form elements:

1. Name input (text)
2. Party size input (number)
3. Submit button. When clicked, the party is added to the waitlist queue.

**Checking in and starting the service**

When the queued party is ready to begin service, the app should display a "check in" button. When clicked:

- The party is removed from the waitlist queue.
- The number of seats available should be decreased by the party size.
- The service countdown starts for that party.

Importantly, the user _must_ be able to view the state of their queued party across multiple browser sessions.

**Queue management**

When a party completes service:

- The system checks the queue for the next party.
- If the seats available are enough for the next party size, the next party’s app shows a new “Check-in” button.
- If not, wait until enough seats are available.

## Submission Guidelines

1. Create a public GitHub repository for your project.
2. Include this README in your repository, with clear instructions for setting up and running the project locally.
3. Include a brief explanation of your architecture decisions in the README or a separate document.

Please grant access to your repo for these following GitHub users

- `daniellizik` - Daniel Lizik, Engineering Manager
- `LuginaJulia` - Julia Lugina, Senior Software Engineer

## Evaluation Criteria

Your submission will be evaluated based on:

1. Functionality: Does the application work as specified?
2. Code Quality: Is the code well-structured, readable, and maintainable? Add sufficient comments in places where you
   think it would help other contributors to onboard more quickly to understand your code.
3. Architecture: Are there clear separations of concerns and good design patterns used?
4. Customer Focus: Is the user experience intuitive? Would _you_ use this application if you were a diner? _Please_ play
   around with your app as if you were a customer prior to submission.
5. QA: Are you confident in the quality of your product? If you had to refactor or add new features, would you be able
   to do so without breaking the existing functionality? There is no guideline on how many tests you should write, what
   type of tests you should write, what level of coverage you need to achieve, etc. We leave it to you to decide how to
   ensure a level of quality that results in your customers trusting your product.

### Good luck!
