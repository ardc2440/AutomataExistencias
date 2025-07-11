los transito son loas ordenes en estado I
/*
CREATE TABLE STRANSITO
(
  ID INTEGER NOT NULL CONSTRAINT PK_STRANSITO PRIMARY KEY, 
  IDITEMTRANSITO Integer NOT NULL,
  FECHAESTRECIBO TIMESTAMP,
  CANTIDADREC INT,
  FECHA TIMESTAMP,
  ACTIVIDAD VARCHAR(200),
  IDITEMXCOLOR INT,
  ACCION CHAR(1) NOT NULL
);
CREATE GENERATOR GEN_STRANSITO;
GRANT DELETE, INSERT, REFERENCES, SELECT, UPDATE
 ON STRANSITO TO  SYSDBA WITH GRANT OPTION;

SET TERM ^ ;


CREATE TRIGGER NEW_STRANSITO FOR STRANSITO
ACTIVE BEFORE INSERT POSITION 0
AS  
BEGIN   
    New.ID = GEN_ID(GEN_STRANSITO,1);
END^
*/
CREATE TRIGGER ACT_STRANSITO FOR ITEMORDEN
ACTIVE AFTER INSERT OR UPDATE OR DELETE POSITION 0
AS  
BEGIN        
    IF (DELETING) THEN 
    BEGIN 
      INSERT INTO STRANSITO (IdUnidad, Accion)
           VALUES(Old.IDUNIDAD, 'D');
    END 
    ELSE
    BEGIN   
        INSERT INTO STRANSITO (IDUNIDAD, NOMUNIDAD, Accion)
             SELECT New.*, 'I'
               FROM ITEMORDEN a 
               JOIN ORDENES b ON b.IDORDEN = a.IDORDEN               
               LEFT JOIN ACTORDEN c ON c.IDORDEN = a.IDORDEN               
              WHERE a.IDITEM = New.IDITEMORDEN; 
    END            
END^

/*SET TERM ; ^ */