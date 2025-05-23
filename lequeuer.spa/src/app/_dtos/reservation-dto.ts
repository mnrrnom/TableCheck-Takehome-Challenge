import {SeatStatus} from '../_dictionary/reservation-status';

export type ReservationDto = {
    id: number,
    leadGuestName: string,
    numberOfDiners: number,
    seatingStatus: SeatStatus,
    createdAt: string;
    restaurantId: number,
}
