function [ordem] = determina_ordem(u,y,k) %Determina a ordem do sistema linear a partir dos dados de entrada e saída
    ordem = 0;
    i=k;% i=numero de linhas;
    [l,Ndados] = size(y); [m,Ndados] = size(u); j = Ndados-2*i; 
    kk = 0;
    % Montando as matrizes em blocos de Hankel
    for k = 1:m:2*i*m-m+1
        kk = kk+1; 
        U(k:k+m-1,:) = u(:,kk:kk+j-1); % Matriz de dados U
    end
    kk = 0;
    for k = 1:l:2*i*l-l+1
        kk = kk+1;
        Y(k:k+l-1,:) = y(:,kk:kk+j-1); % Matriz de dados Y
    end
    Uf = U(i*m+1:2*i*m,:); % entradas futuras
    Yf = Y(i*l+1:2*i*l,:); % saídas futuras
    Up = U(1:i*m,:); % entradas passadas
    Yp = Y(1:i*l,:); % saídas passadas
    Wp = [Up; Yp]; % empilhando Up e Yp
    H = [Uf;Yf]; % matriz de dados final
    % Decomposição LQ
    L = triu(qr([H]'))';
    im=i*m;
    il=i*l;
    L11 = L(1:im,1:im);
    L21 = L(im+1:im+il,1:im);
    L22 = L(im+1:im+il,im+1:im+il);
    % Decomposição em valores singulares
    [UU,SS,VV] = svd(L22);
    for j=1:1:size(SS,1)
        for k=1:1:size(SS,2)
            if SS(j,k)> 10^-5
                ordem = ordem + 1; %Ordem do sistema
            end
        end
    end
end