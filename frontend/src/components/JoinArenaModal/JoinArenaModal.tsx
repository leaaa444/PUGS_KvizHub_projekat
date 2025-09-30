import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import Modal from '../Modal/Modal';
import './JoinArenaModal.css'

interface JoinArenaModalProps {
    isOpen: boolean;
    onClose: () => void;
}

const JoinArenaModal: React.FC<JoinArenaModalProps> = ({ isOpen, onClose }) => {
    const navigate = useNavigate();
    const [roomCode, setRoomCode] = useState('');
    const [error, setError] = useState(''); 
    useEffect(() => {
        if (isOpen) {
            setRoomCode('');
            setError('');
        }
    }, [isOpen]);

    const handleJoin = (e: React.FormEvent) => {
        e.preventDefault();
        
        if (!roomCode.trim()) {
            setError('Morate uneti kod sobe.');
            return;
        }

        const formattedRoomCode = roomCode.trim().toUpperCase();
        
        onClose();
        
        navigate(`/live-arena/lobby/${formattedRoomCode}`);
    };

    return (
        <Modal isOpen={isOpen} onClose={onClose}>
        <div className="join-arena-content">
            <h2>Pridruži se Areni</h2>
            <form onSubmit={handleJoin} className="join-arena-form">
                <input
                    type="text"
                    value={roomCode}
                    onChange={e => setRoomCode(e.target.value)}
                    placeholder="Unesi kod sobe"
                    autoFocus
                    required
                    maxLength={5} 
                    className="join-arena-input"
                />
                <button type="submit" className="join-arena-button">Pridruži se</button>
                <p className="join-arena-error">{error || ' '}</p>
            </form>
        </div>
    </Modal>
    );
};

export default JoinArenaModal;