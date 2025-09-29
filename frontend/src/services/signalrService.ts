import * as signalR from "@microsoft/signalr";
import { GameRoom, QuestionDto } from '../features/live-quiz/types';

const BASE_URL  = process.env.REACT_APP_API_BASE_URL;
if (!BASE_URL) {
    throw new Error("REACT_APP_API_BASE_URL  nije definisan! Proveri .env fajl.");
}
const hubUrl = `${BASE_URL}/quizhub`;

const connection = new signalR.HubConnectionBuilder()
    .withUrl(hubUrl, {
        accessTokenFactory: () => localStorage.getItem("user_token") || ""
    })
    .withAutomaticReconnect()
    .build();

// Promenljiva koja će čuvati obećanje o konekciji
let startPromise: Promise<void> | null = null;

export const startConnection = () => {
    if (connection.state === signalR.HubConnectionState.Connected) {
        return Promise.resolve();
    }

    if (startPromise) {
        return startPromise;
    }

    startPromise = connection.start()
        .then(() => {
            console.log("SignalR konekcija je uspešno uspostavljena.");
            // Resetuj promise kada se uspešno povežemo
            startPromise = null;
        })
        .catch(err => {
            console.error("Greška pri povezivanju sa SignalR:", err);
            // Resetuj promise i u slučaju greške, da bi se moglo pokušati ponovo
            startPromise = null; 
            // Prosledi grešku dalje kako bi je pozivalac mogao obraditi
            throw err;
        });
        
    return startPromise;
};

export const createRoom = (quizId: number) => {
    // Čekamo da se konekcija uspostavi pre nego što pozovemo metodu
    return startConnection().then(() => {
        return connection.invoke("CreateRoom", quizId);
    }).catch(err => console.error("Greška pri pozivanju CreateRoom:", err));
};

export const enterLobby = (roomCode: string) => {
    return startConnection().then(() => connection.invoke("EnterLobby", roomCode));
};

export const startGame = (roomCode: string) => {
    return startConnection().then(() => connection.invoke("StartGame", roomCode));
};

export const submitAnswer = (answerDto: { roomCode: string, questionId: number, selectedOptionIds: number[], textAnswer: string | null }) => {
    return startConnection().then(() => 
        connection.invoke("SubmitAnswer", answerDto)
    );
};

let roomSubscribers: ((room: GameRoom) => void)[] = [];

connection.on("UpdateRoom", (room) => {
    console.log(`%c[SIGNALR SERVIS] PRIMLJEN 'UpdateRoom'. Obaveštavam ${roomSubscribers.length} pretplatnika.`, 'color: #00DD00; font-weight: bold;', room);
    roomSubscribers.forEach(callback => callback(room));
});

export const subscribeToRoomUpdates = (callback: (room: GameRoom) => void) => {
    roomSubscribers.push(callback);
    console.log('[signalrService] Komponenta se prijavila na "UpdateRoom" događaj.');
    
    return () => {
        roomSubscribers = roomSubscribers.filter(sub => sub !== callback);
        console.log('[signalrService] Komponenta se odjavila sa "UpdateRoom" događaja.');
    };
};

export const onRoomCreated = (callback: (roomCode: string) => void) => {
    connection.on("RoomCreated", callback);
    return () => connection.off("RoomCreated", callback);
};

export const onError = (callback: (message: string) => void) => {
    connection.on("Error", callback);
    return () => connection.off("Error", callback);
};


export const onHostDisconnected = (callback: (message: string) => void) => {
    connection.on("HostDisconnected", callback);
    return () => connection.off("HostDisconnected", callback);
};

export const onGameStarted = (callback: () => void) => {
    connection.on("GameStarted", callback);
    return () => connection.off("GameStarted", callback);
};

export const onNewQuestion = (callback: (question: QuestionDto) => void) => {
    connection.on("NewQuestion", callback);
    return () => connection.off("NewQuestion", callback);
};

export const onGameFinished = (callback: (finalRoomState: GameRoom) => void) => {
    connection.on("GameFinished", callback);
    return () => connection.off("GameFinished", callback);
};

export const onRoomClosed = (callback: (message: string) => void) => {
    connection.on("RoomClosed", callback);
    return () => connection.off("RoomClosed", callback);
};

export const disconnect = async () => {
    if (connection && connection.state === 'Connected') {
        try {
            await connection.stop();
            console.log('SignalR veza je uspešno prekinuta.');
        } catch (err) {
            console.error('Greška pri prekidanju SignalR veze:', err);
        }
    }
};


export const stopSignalRConnection = () => {
    if (connection.state === signalR.HubConnectionState.Connected) {
        connection.stop();
        console.log("SignalR konekcija zaustavljena.");
    }
};

export { connection };
