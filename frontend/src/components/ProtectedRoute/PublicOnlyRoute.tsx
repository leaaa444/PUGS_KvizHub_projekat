import React from 'react';
import { Navigate } from 'react-router-dom';
import { useAuth } from '../../context/AuthContext';

interface PublicOnlyRouteProps {
  children: React.ReactNode;
}

const PublicOnlyRoute: React.FC<PublicOnlyRouteProps> = ({ children }) => {
  const { user } = useAuth()

  if (user) {
    return <Navigate to="/kvizovi" replace />;
  }

  return <>{children}</>;
};

export default PublicOnlyRoute;