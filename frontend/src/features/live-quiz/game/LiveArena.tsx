import React, { useState, useEffect, useRef, useCallback } from 'react';
import { useParams, useNavigate } from 'react-router-dom';
import { onNewQuestion, onGameFinished, subscribeToRoomUpdates, onGameStarted, submitAnswer, enterLobby, disconnect } from '../../../services/signalrService';
import { GameRoom, QuestionDto } from '../types';
import LiveQuestionArea from '../../quiz/components/LiveQuestionArea';
import QuizHeader from '../../quiz/components/QuizHeader';
import LiveLeaderboard from './LiveLeaderboard';
import { useAuth } from '../../../context/AuthContext';
import Modal from '../../../components/Modal/Modal';
import './LiveArena.css';

const READING_DURATION_MS = 5000;

const LiveArena = () => {
    const { roomCode } = useParams<{ roomCode: string }>();
    const navigate = useNavigate();
    const { user } = useAuth();
    
    const [room, setRoom] = useState<GameRoom | null>(null);
    const [currentQuestion, setCurrentQuestion] = useState<QuestionDto | null>(null);
    const [timer, setTimer] = useState(0);
    const [phase, setPhase] = useState<'waiting' | 'reading' | 'answering' | 'finished'>('waiting');
    const [answered, setAnswered] = useState(false); 
    const [currentAnswer, setCurrentAnswer] = useState<any>(null);
    const [isSubmitting, setIsSubmitting] = useState(false);
    const [totalQuestions, setTotalQuestions] = useState(0); 

    const [showExitConfirm, setShowExitConfirm] = useState(false);

    const timerIntervalRef = useRef<NodeJS.Timeout | null>(null);
    const isHost = room?.hostUsername === user?.username;

    useEffect(() => {
        if (room) {
            console.log('%c[LiveArena] Stanje "room" je AŽURIRANO. Trenutni broj igrača:', 'color: purple; font-weight: bold;', room.players.length);
            
            console.log('Svi igrači (detaljno):', JSON.stringify(room.players, null, 2));
        } else {
            console.log('%c[LiveArena] Stanje "room" je postavljeno na null.', 'color: purple; font-weight: bold;');
        }
    }, [room])

    useEffect(() => {
        if (!roomCode) return;
        

        const handleNewQuestion = (question: QuestionDto) => {
            console.log('NOVO PITANJE PRIMLJENO:', question);
            if (timerIntervalRef.current) clearInterval(timerIntervalRef.current);

            setCurrentQuestion(question);
            setTotalQuestions(question.totalQuestions);
            setCurrentAnswer(null); 
            setAnswered(false);

            if (question.remainingTime !== undefined) {
                setTimer(Math.floor(question.remainingTime));
                setPhase('answering');
            } else {
                setTimer(question.timeLimit || 15);
                setPhase('reading');
            }
        };

        const handleGameFinished = (finalRoomState: GameRoom) => {
            console.log("KVIZ JE ZAVRŠEN!", finalRoomState);
            if (timerIntervalRef.current) clearInterval(timerIntervalRef.current);
            setRoom(finalRoomState);
            setPhase('finished');

        };

        const unsubscribeRoomUpdates = subscribeToRoomUpdates((updatedRoom) => setRoom(updatedRoom));
        const unsubscribeNewQuestion = onNewQuestion(handleNewQuestion);
        const unsubscribeGameFinished = onGameFinished(handleGameFinished);
        const unsubscribeGameStarted = onGameStarted(() => console.log("Igra je počela!"));

        console.log(`%c POZIVAM 'EnterLobby' za sobu: ${roomCode}`, 'color: yellow');
        enterLobby(roomCode).catch(err => {
            console.error(`Greška pri pozivanju EnterLobby:`, err);
            navigate('/kvizovi'); 
        });

        return () => {
            console.log('%c[LiveArena Cleanup] Komponenta se uništava. Odjavljujem se sa događaja.', 'color: orange;');

            disconnect();
            unsubscribeGameStarted();
            unsubscribeNewQuestion();
            unsubscribeGameFinished();
            unsubscribeRoomUpdates();
        };
    }, [roomCode, navigate]);
    
    useEffect(() => {
        if (phase === 'reading') {
            const transitionTimeout = setTimeout(() => {
                console.log("Faza čitanja je istekla. Prelazim na 'answering'.");
                setPhase('answering');
            }, READING_DURATION_MS);
            return () => clearTimeout(transitionTimeout);
        }
    }, [phase]);

    useEffect(() => {
        if (phase !== 'answering') {
            return;
        }

        timerIntervalRef.current = setInterval(() => {
            setTimer(prev => {
                if (prev <= 1) {
                    if(timerIntervalRef.current) clearInterval(timerIntervalRef.current);
                    return 0;
                }
                return prev - 1;
            });
        }, 1000);

        return () => {
            if (timerIntervalRef.current) {
                clearInterval(timerIntervalRef.current);
            }
        };
    }, [phase]);


    const handleSubmitAnswer = useCallback(() => {
        if (isSubmitting || !currentQuestion || currentAnswer === null || !roomCode) {
            return;
        }
        if (isHost) {
            console.warn("Host ne može da odgovara.");
            return;
        }

        setIsSubmitting(true);
        setAnswered(true);
        
        const type = currentQuestion.type;
        let selectedIds: number[] = [];
        let textAns: string | null = null;

        if (type === 'FillInTheBlank') {
            textAns = currentAnswer;
        } else {
            selectedIds = Array.isArray(currentAnswer) ? currentAnswer : [currentAnswer];
        }
        
        const answerDto = {
            roomCode,
            questionId: currentQuestion.questionID,
            selectedOptionIds: selectedIds,
            textAnswer: textAns,
        };

        submitAnswer(answerDto)
            .then(() => console.log(`[SignalR] USPEŠNO POSLAT ODGOVOR.`))
            .catch(err => console.error("Greška pri slanju odgovora:", err))
            .finally(() => setIsSubmitting(false));
    }, [isSubmitting, currentQuestion, currentAnswer, roomCode, isHost]);

    const handleReturnToLobby = () => {
        navigate('/kvizovi');
    };

    
    const handleConfirmExit = () => navigate('/kvizovi');
    const handleCancelExit = () => setShowExitConfirm(false);

    if (!room) {
        return <div className="quiz-page-container"><h2>Učitavanje sobe...</h2></div>;
    }
    return (
        <>
            <div className="quiz-page-container">
                

                <QuizHeader name={`Soba: ${room.roomCode}`} timeLeft={timer} />
                <div className="game-area">
                    <div className="question-display">
                        {phase === 'waiting' && <h2>Sačekajte sledeće pitanje...</h2>}
                        {phase === 'reading' && <h2>Pitanje sledi...</h2>}
                        
                        {isHost && phase === 'answering' && <h4 style={{textAlign: 'center', margin: '20px'}}>Vi ste host, nadgledate igru.</h4>}

                        {currentQuestion && phase !== 'finished' && (
                            <>
                                <LiveQuestionArea
                                    currentQuestion={currentQuestion}
                                    showOptions={!isHost && phase === 'answering' && !answered}
                                    currentAnswer={currentAnswer} 
                                    currentQuestionIndex={currentQuestion.currentQuestionIndex}
                                    totalQuestions={totalQuestions}
                                    onAnswerChange={setCurrentAnswer}
                                />
                                {phase === 'answering' && !isHost && !answered && (
                                    <div className="submit-answer-wrapper">
                                        <button 
                                            onClick={handleSubmitAnswer} 
                                            disabled={currentAnswer === null || isSubmitting}
                                            className="submit-answer-btn"
                                        >
                                            {isSubmitting ? 'Slanje...' : 'Potvrdi odgovor'}
                                        </button>
                                    </div>
                                )}
                            </>
                        )}
                
                        {phase === 'finished' && (
                            <div className="quiz-finished-container">
                                <h2>Kviz je završen!</h2>

                                <LiveLeaderboard 
                                    title="Finalni rezultati:" 
                                    players={room.players}
                                    containerClassName="final-leaderboard"
                                />

                                <button onClick={handleReturnToLobby} className="submit-answer-btn">
                                    Nazad na listu kvizova
                                </button>
                            </div>
                        )}
                    </div>
                    
                {phase !== 'finished' && (
                    <button onClick={() => setShowExitConfirm(true)} className="exit-quiz-btn">
                        Izađi iz kviza
                    </button>
                )}
                
                    {phase !== 'finished' && (
                        <LiveLeaderboard 
                            title="Rang Lista" 
                            players={room.players} 
                        />
                    )}
                </div>
                
            </div>
            <Modal isOpen={showExitConfirm} onClose={handleCancelExit}>
                <div className="exit-confirm-content" style={{ padding: '20px', textAlign: 'center' }}>
                    <h3>Da li ste sigurni?</h3>
                    <p>Ako napustite stranicu, bićete diskonektovani iz sobe i nećete moći da nastavite kviz.</p>
                    <div className="modal-actions" style={{ marginTop: '20px', display: 'flex', justifyContent: 'center', gap: '10px' }}>
                        <button type="button" onClick={handleConfirmExit} className="btn-confirm-exit">
                            Da, izađi
                        </button>
                        <button onClick={handleCancelExit} className="btn-cancel-exit">
                            Ne, ostajem
                        </button>
                    </div>
                </div>
            </Modal>
        </>
    );
};
export default LiveArena;

