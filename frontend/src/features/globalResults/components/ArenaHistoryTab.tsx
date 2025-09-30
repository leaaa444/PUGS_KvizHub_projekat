import React, { useState, useEffect } from 'react';
import { Link } from 'react-router-dom';
import liveResultService from '../../../services/liveResultService';
import Modal from '../../../components/Modal/Modal';
import { GameRoom } from '../../live-quiz/types';
import LiveLeaderboard from '../../live-quiz/game/LiveLeaderboard';

interface FinishedArena {
    roomCode: string;
    quizName: string;
    hostUsername: string;
    finishedAt: string; 
    participantCount: number;
    winnerUsername: string;
    winnerScore: number;
}

const ArenaHistoryTab: React.FC = () => {
    const [arenas, setArenas] = useState<FinishedArena[]>([]);
    const [loading, setLoading] = useState(true);

    const [isModalOpen, setIsModalOpen] = useState(false);
    const [selectedArenaDetails, setSelectedArenaDetails] = useState<GameRoom | null>(null);
    const [modalLoading, setModalLoading] = useState(false);

    useEffect(() => {
        liveResultService.getFinishedArenas()
            .then(response => {
                setArenas(response.data);
            })
            .catch(error => {
                console.error("Greška pri preuzimanju istorije arena:", error);
            })
            .finally(() => setLoading(false));
    }, []);

    const handleShowDetails = (roomCode: string) => {
        setIsModalOpen(true);
        setModalLoading(true);
        liveResultService.getArenaDetails(roomCode)
            .then(response => {
                setSelectedArenaDetails(response.data);
            })
            .catch(error => {
                console.error("Greška pri preuzimanju detalja arene:", error);
                setIsModalOpen(false);
            })
            .finally(() => {
                setModalLoading(false);
            });
    };

    const handleCloseModal = () => {
        setIsModalOpen(false);
        setSelectedArenaDetails(null);
    };

    if (loading) {
        return <p>Učitavanje istorije...</p>;
    }

    return (
        <>
            <div className="arena-history-container">
                {arenas.length > 0 ? (
                    arenas.map(arena => (
                        <div key={arena.roomCode} className="arena-card">
                            <div className="arena-card-header">
                                <h3>{arena.quizName}</h3>
                                <span>Odigrano: {new Date(arena.finishedAt).toLocaleDateString('sr-RS')}</span>
                            </div>
                            <div className="arena-card-body">
                                <p><strong>Domaćin:</strong> {arena.hostUsername}</p>
                                <p><strong>Broj igrača:</strong> {arena.participantCount}</p>
                                <p className="winner">
                                    🏆 <strong>Pobednik:</strong> {arena.winnerUsername} ({arena.winnerScore} poena)
                                </p>
                            </div>
                            <div className="arena-card-footer">
                                <button onClick={() => handleShowDetails(arena.roomCode)} className="details-link">
                                        Detalji partije
                                </button>
                            </div>
                        </div>
                    ))
                ) : (
                    <p>Nema odigranih arena za prikaz.</p>
                )}
            </div>

            <Modal isOpen={isModalOpen} onClose={handleCloseModal}>
                <div className="arena-details-modal">
                    {modalLoading && <p>Učitavanje rezultata...</p>}
                    
                    {!modalLoading && selectedArenaDetails && (
                        <>
                            <h2 className="details-title">Rezultati Arene</h2>
                            <div className="details-subtitle">
                                <span>Kviz: <strong>{selectedArenaDetails.quizName || 'Nepoznat Kviz'}</strong></span>
                                <span>Kod sobe: <strong>{selectedArenaDetails.roomCode}</strong></span>
                            </div>

                            <LiveLeaderboard
                                title="Finalni Poredak"
                                players={selectedArenaDetails.players}
                                containerClassName="final-leaderboard-modal"
                            />
                        </>
                    )}
                </div>
            </Modal>
        </>
    );
};

export default ArenaHistoryTab;