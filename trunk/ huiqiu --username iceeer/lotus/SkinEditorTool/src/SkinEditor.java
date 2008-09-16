/* Pawan Gupta
 * Domino Web Access    
 * Date:    06/05/2003
 *
 * This program is designed to run locally.  You will want to either shut down your Domino
 * server, or restart the HTTP server after making changes.
 *
 * Once you open a Forms6.nsf (or Forms5.nsf) database, this tool will display a list of 
 * the "skin groups".  There is a skin group for IE, for Mozilla, and for other browsers.
 * Choosing a skin group, will populate the "skins" selection control.  Select one of these
 * "skins" to view the HTML or CSS content.
 *
 * You may now edit and save changes.  The type of changes you can make are:
 *  1. Placement of objects on the view pages
 *  2. Placement of objects on the form pages ("new")
 *  3. Color and style of all objects on the page (via CSS rules in the StyleSheet.css document)
 *
 */
    
import java.io.*;
import java.lang.*;
import java.util.*;
import java.awt.*;
import java.awt.event.*;
import java.awt.datatransfer.*;
import javax.swing.*;
import lotus.domino.*;



class SkinEditor extends JFrame implements ClipboardOwner{

    private static String       m_Title  = "DominoWebAccess Skin Editor";

    private static Session      m_ns;
    private static Database     m_db;
    private static String       m_FormsFilepath;
    private static JTextArea    m_txtArea;
    private static JButton      m_update;
    private static JComboBox    m_cbxBrowserSkins, m_cbxDWASkins;
    private static JLabel       m_status;
    private static Document     m_doc;

    private static Hashtable    m_hashDocUNID;

    private static Vector       m_systemSkinGroupUNID;
    private static Vector       m_systemSkinName;
    private static Vector       m_systemSkinUNID;
    
    private static Vector       m_vSkinGroup;
    private static Vector       m_vSkinData;


    // ================================================================
    public static void exit()
    //
    // exit the application
    // ================================================================
    {
        System.exit(0);
    }

    // ================================================================
    public static void main(String[] args)
    //
    // launch the application
    // ================================================================
    {
        try{
            
            // Establish a NotesSession
            NotesThread.sinitThread();
            Session ns = NotesFactory.createSession();

            // Init the editor object
            SkinEditor ss = new SkinEditor(ns);
    
            // Get the Forms file
            if( args.length > 0 ){
                m_FormsFilepath = args[0];
            }
            else{
                m_FormsFilepath = SkinEditor.openFile(ss);
            }
    
            // Launch the Editor
            if( !m_FormsFilepath.equals("") ){
                ss.showEditor();

                // Loop so that the application doesn't exit
                while( ss.isDisplayable() ){
                    Thread.sleep(2000);
                }
            }
            else{
                print("No Forms file specified.");
            }
        }
        catch(Exception ex){
            ex.printStackTrace();
        }

        finally {
            NotesThread.stermThread();
            exit();
        }
    }

    // ================================================================
    public SkinEditor(final Session ns)
    // 
    // this is the initialization method of the SkinEditor class
    // ================================================================
    {
        super(m_Title);

        // Session handle
        m_ns = ns;
        
        // Exit strategy
        this.setDefaultCloseOperation( WindowConstants.DISPOSE_ON_CLOSE );

        // Design Layout 
        GridBagConstraints gbCon = new GridBagConstraints();
        GridBagLayout gbLayout   = new GridBagLayout();
        Container cnt            = getContentPane();
        cnt.setLayout( gbLayout );
        
        Dimension dim = Toolkit.getDefaultToolkit().getScreenSize();    

        JButton btn;
        JLabel  lbl;
        int btnW  = 100;
        int btnH  = 20;
        int gridW = 1;
        int gridH = 1;
        
        // ----------------------------
        // Status
        // ----------------------------
        m_status = new JLabel("Initializing, please wait", JLabel.CENTER);
        m_status.setFont(new Font("Serif", Font.BOLD, 40));
        addComponent( cnt, gbCon, gbLayout, m_status, 5, 0, 5, 10 );

        // ----------------------------
        // Tool Title
        // ----------------------------
        lbl = new JLabel("Domino Web Access" , JLabel.CENTER);
        lbl.setFont(new Font("Serif", Font.BOLD, 30));
        addComponent( cnt, gbCon, gbLayout, lbl, 1, 0, 0, 1 );

        lbl = new JLabel("Skin Editor Tool", JLabel.CENTER);
        lbl.setFont(new Font("Serif", Font.BOLD + Font.ITALIC, 25));
        addComponent( cnt, gbCon, gbLayout, lbl, 2, 0, 0, 1);


        // ----------------------------
        // ComboBoxes
        // ----------------------------

        //    create a new List    which    contain    all    the    skin names
        lbl = new JLabel("Browser Skins ");
        addComponent( cnt, gbCon, gbLayout, lbl, 3, 0, 1, 1);
        m_cbxBrowserSkins = new JComboBox();
        m_cbxBrowserSkins.setMaximumRowCount(4);
        m_cbxBrowserSkins.insertItemAt("Browser Skin", 0);
        addComponent( cnt, gbCon, gbLayout, m_cbxBrowserSkins, 3, 1, 1, 1);

        lbl = new JLabel("DWA Skins ");
        addComponent( cnt, gbCon, gbLayout, lbl, 3, 2, 1, 1 ); 
        m_cbxDWASkins = new JComboBox();
        m_cbxDWASkins.setMaximumRowCount(10);
        addComponent( cnt, gbCon, gbLayout, m_cbxDWASkins, 3, 3, 1, 1 );



        // ----------------------------
        // Editor Pane
        // ----------------------------
        m_txtArea = new JTextArea(30, 65);      // 30, 65
        m_txtArea.setSize(dim);
        m_txtArea.setVisible(false);
        JScrollPane pictureScrollPane = new JScrollPane(m_txtArea);        
        addComponent( cnt, gbCon, gbLayout, pictureScrollPane, 5, 0, 5, 10 );  // 4055

        
        // ----------------------------
        // Button:  Copy
        // ----------------------------
        btn = new JButton("Copy");
        btn.setSize( btnW, btnH );
        gbCon.fill = GridBagConstraints.HORIZONTAL;
        btn.addActionListener(
            new ActionListener(){
                public void actionPerformed(ActionEvent ev){
                    copy();
                }
            }
        );
        addComponent( cnt, gbCon, gbLayout, btn, 5,10, gridW, gridH);

        // ----------------------------
        // Button:  Copy All
        // ----------------------------
        btn = new JButton("Copy All");
        btn.setSize( btnW, btnH );
        gbCon.fill = GridBagConstraints.HORIZONTAL;
        btn.addActionListener(
            new ActionListener(){
                public void actionPerformed(ActionEvent ev){
                    copyAll();
                }
            }
        );
        addComponent( cnt, gbCon, gbLayout, btn, 6,10, gridW, gridH);

        // ----------------------------
        // Button:  Paste
        // ----------------------------
        btn = new JButton("Paste");
        btn.setSize( btnW, btnH );
        gbCon.fill = GridBagConstraints.HORIZONTAL;
        btn.addActionListener(
            new ActionListener(){
                public void actionPerformed(ActionEvent ev){
                    copy();
                }
            }
        );
        addComponent( cnt, gbCon, gbLayout, btn, 7,10, gridW, gridH);            

        // ----------------------------
        // Button:  Paste All
        // ----------------------------
        btn = new JButton("Paste All");
        btn.setSize( btnW, btnH );
        gbCon.fill = GridBagConstraints.HORIZONTAL;
        btn.addActionListener(
            new ActionListener(){
                public void actionPerformed(ActionEvent ev){
                    pasteAll();
                }
            }
        );
        addComponent( cnt, gbCon, gbLayout, btn, 8,10, gridW, gridH);


        // ----------------------------
        // Button:  Update
        // ----------------------------
        m_update = new JButton("Update");
        m_update.setSize( btnW, btnH );
        gbCon.fill = GridBagConstraints.HORIZONTAL;
        m_update.setEnabled(false);
        m_update.addActionListener(
            new ActionListener(){
                public void actionPerformed(ActionEvent ev){
                    updateSkin(ev);
                }
            }
        );
        addComponent( cnt, gbCon, gbLayout, m_update, 9,10, gridW, gridH);


        // ----------------------------
        // Button:  Exit
        // ----------------------------
        btn = new JButton("Exit");
        btn.setSize( btnW, btnH );
        gbCon.fill = GridBagConstraints.HORIZONTAL;
        btn.addActionListener(
            new ActionListener(){
                public void actionPerformed(ActionEvent ev){
                    exit();
                }
            }
        );
        addComponent( cnt, gbCon, gbLayout, btn, 10,10, gridW, gridH);
        
        setSize(dim);
        this.setResizable(true);
    }

    // ================================================================
    private void addComponent( Container cnt, GridBagConstraints cnst, GridBagLayout layout, Component cmp, int row, int column, int width, int height )
    // ================================================================
    {
        // set component placement and size
        cnst.gridx = column;
        cnst.gridy = row;
        cnst.gridwidth = width;   
        cnst.gridheight = height;
        cnst.insets = new Insets(10,10,0,0);
        
        layout.setConstraints( cmp, cnst );  
        cnt.add( cmp );
    }

    // ================================================================
    private void showEditor()
    // ================================================================
    {
        try{
            show(); // superclass method

            m_ns.setConvertMIME(false);
            m_db = m_ns.getDatabase(null, m_FormsFilepath , true);


            // --------------------------------------------
            // Collect all skin groups
            //  and initialize the first ComboBox (Skin Groups)
            // --------------------------------------------
            ViewEntryCollection vc1 = m_db.getView("System\\skin Groups").getAllEntries();
            ViewEntry entry1        = vc1.getFirstEntry();
            m_vSkinGroup            = new Vector();
			m_vSkinGroup.add(0, "Null String");
            while( null != entry1 ){
                Vector v1 = new Vector();
                v1 = entry1.getColumnValues();
				
			    m_vSkinGroup.addElement(v1.elementAt(0));
				m_cbxBrowserSkins.addItem(v1.elementAt(1));
				entry1 = vc1.getNextEntry();
            }

            m_cbxBrowserSkins.addItemListener(
                new ItemListener(){
                    public void itemStateChanged(ItemEvent ev){
                        handleCbxSelection( ev );
						
                    }
                }
            );


            // --------------------------------------------
            // Collect all skin types
            //  and initialize the second ComboBox (Skins)
            // --------------------------------------------
            
            // Contains the UNIDs of all the documents in the System\\Skins view
            m_systemSkinGroupUNID   = new Vector();
            // Contains all the skin names
            m_systemSkinName        = new Vector();
            m_systemSkinUNID        = new Vector();

            // Hashtable:
            //   keys:      skin UNIDs
            //   values:    skin document objects
            m_hashDocUNID = new Hashtable();

            ViewEntryCollection vc2 = m_db.getView("System\\skins").getAllEntries();
            ViewEntry entry2        = vc2.getFirstEntry();
            while( null != entry2 ){
                Vector v2 = new Vector();
                v2 = entry2.getColumnValues();    
                
                // Skin Name
                m_systemSkinName.addElement(v2.elementAt(1));
                
                // Skin Group UNID (The skin group that this individual skin is associated with)
                m_systemSkinGroupUNID.addElement(v2.elementAt(0));
                
                // Skin UNID
                m_systemSkinUNID.addElement(v2.elementAt(3));
                
                // add the key and value in    hash table.    
                // first argument    is key ( skin    names    )    second argument    is documents.    
                m_hashDocUNID.put(v2.elementAt(3), entry2.getDocument()) ;
                
                // get the next    entry    
                entry2 = vc2.getNextEntry();
            }

            m_cbxDWASkins.addItemListener(
                new ItemListener(){
                    public void itemStateChanged(ItemEvent ev){
                        handleCbxSelection(ev);
                    }
                }
            );

            // allow editing to begin
            m_status.setVisible(false);
            m_txtArea.setVisible(true);
        }
        catch (Exception    e){
            e.printStackTrace();
        }
    }
    
    // ================================================================
    public static void handleCbxSelection( ItemEvent ev )
    // ================================================================
    {
        if( ItemEvent.SELECTED == ev.getStateChange() ){

            // ------------------------
            // Skin Group Change
            // ------------------------
            if( m_cbxBrowserSkins == ev.getItemSelectable() ){
                
                // reset UI elements
                resetUI();
                // get the selected skin group
                String skinGroup = m_cbxBrowserSkins.getSelectedItem().toString();

                
                if( skinGroup.equals(" ") || skinGroup.equals("Browser Skin") ){

				    JOptionPane.showMessageDialog(null,	"Invalid choice in Browser Skin ComboBox",	"Error",	
				                  JOptionPane.ERROR_MESSAGE);
                    return;
                }

                populateSkinComboBox((String)m_vSkinGroup.elementAt( m_cbxBrowserSkins.getSelectedIndex()));

                // done processing event
                return;
            }
            
            // ------------------------
            // Skin Change
            // ------------------------
            if( m_cbxDWASkins == ev.getItemSelectable() ){

                // Get the selected Skin name
                String skinName = m_cbxDWASkins.getSelectedItem().toString();
                if( skinName.equals(" ") || skinName.equals("Select Skin") ){
                    alert("Please select a skin name");
					//alert("Please select a skin name", JOptionPane.ERROR_MESSAGE);

                    return;
                }

                int idx         = m_cbxDWASkins.getSelectedIndex();
                String sUNID    = m_vSkinData.elementAt(idx).toString();
                Document ndoc   = (Document)m_hashDocUNID.get(sUNID);
                
                loadSkin(ndoc);
 
                // done handling event
                return;
            }
        }
    }
    
    // ================================================================
    public static void populateSkinComboBox(final String skinGroup)
    // ================================================================
    {
        //Vector addlistdata;
        
        //addlistdata = new Vector();
        m_vSkinData = new Vector();

        m_cbxDWASkins.removeAllItems();

        m_cbxDWASkins.insertItemAt("Select Skin", 0);

        m_vSkinData.add(0, "Null String");
   		
        for(int b=0; b<m_systemSkinGroupUNID.size(); b++){
            if( skinGroup.equals(m_systemSkinGroupUNID.elementAt(b)) ){
                //addlistdata.addElement(m_systemSkinName.elementAt(b));
                m_cbxDWASkins.addItem(m_systemSkinName.elementAt(b));
            }
        }

        for(int b=0; b<m_systemSkinUNID.size(); b++){
            if( skinGroup.equals(m_systemSkinGroupUNID.elementAt(b)) ){
                m_vSkinData.addElement(m_systemSkinUNID.elementAt(b));
            }
        }
    }

    // ================================================================
    public static void resetUI()
    // ================================================================
    {
        // reset UI elements
        m_doc = null;
        m_update.setEnabled(false);
        m_txtArea.setText("");
    }

    // ================================================================
    public static void loadSkin(Document doc)
    // ================================================================
    {
        int iStatus=0;
        try{
            m_doc = doc;
            NotesThread.sinitThread();
            final MIMEEntity mime   = m_doc.getMIMEEntity("h_Body");
            boolean bSet = false;

            // use MIME object to extract HTML as text
            if( mime != null ){
                m_txtArea.setText( mime.getContentAsText() );
                iStatus=1;
            }
        }
        catch(Exception ex){
            ex.printStackTrace();
            iStatus=-1;
        }
        finally{
            NotesThread.stermThread();
            
            String sMsg="";
            switch (iStatus){
                case -1:
                    sMsg = "\nSee stack trace in console window.";
                case  0:
                    resetUI();
                    alert("This skin is empty.");
                    break;
                case  1:
                    m_update.setEnabled(true);
                    break;
            }
        }
    }

    // ================================================================
    public static void updateSkin(ActionEvent ev)
    // ================================================================
    {
        int iStatus=0;
        try{
            NotesThread.sinitThread();
            
            Stream strm        = m_ns.createStream();
            MIMEEntity mime    = m_doc.getMIMEEntity("h_Body") ;
          //  String contentType = mime.getContentType();
          //  int encodingType   = mime.getEncoding();

		    // convert HTML text to MIME
            strm.writeText( m_txtArea.getText() );
		    mime.setContentFromText( strm, "text/html;charset=UTF-8", MIMEEntity.ENC_IDENTITY_7BIT );
            strm.close();

            //save the document.    
            if( m_doc.save(true, true) ){
                iStatus=1;
            }
        }
        catch(Exception e){
            e.printStackTrace();
            iStatus=-1;
        }
        finally{
            NotesThread.stermThread();
            
            String sMsg="";
            switch (iStatus){
                case -1:
                    sMsg = "\nSee stack trace in console window.";
                case  0:
                    alert("Error updating the document.");
                    break;
                case  1:
                    alert( "Document has been updated." );
                    break;
            }
        }
    }

    // ================================================================
    public static String openFile(SkinEditor ss)
    //
    // returns the name of the file chosen
    // ================================================================
    {
        String filePath = "";
        JFileChooser fileChooser = new JFileChooser();
        fileChooser.setFileSelectionMode( JFileChooser.FILES_ONLY );
        int result = fileChooser.showOpenDialog(ss);
        
        // If user cancelled, return null
        if( JFileChooser.CANCEL_OPTION != result ){
            File fc = fileChooser.getSelectedFile();
            return fc.toString();
        }
        return null;
    }

    // ================================================================
    public static void alert( final String sMsg, int iType )
    //
    // a generic MessageBox routine
    // ================================================================
    {
        JOptionPane.showMessageDialog(null, sMsg, m_Title, iType );
    }
    public static void alert( final String sMsg ){
        alert(sMsg, JOptionPane.INFORMATION_MESSAGE);
    }

    // ================================================================
    public static void print( final String sMsg )
    //
    // a generic "print" routine
    // ================================================================
    {
        System.out.println(sMsg);
    }
        
    // ================================================================
    public void lostOwnership(Clipboard c, Transferable t)
    // 
    // necessary for implementing the ClipboardOwner interface
    // ================================================================
    {
        m_txtArea.select(0,0);
    }



/*    
 * Copyright (c) 2000 David Flanagan. All rights reserved.
 * This code is from the book Java Examples in a Nutshell, 2nd Edition.
 * It is provided AS-IS, WITHOUT ANY WARRANTY either expressed or implied.
 * You may study, use, and modify it for any non-commercial purpose.
 * You may distribute it non-commercially as long as you retain this notice.
 * For a commercial use license, or to purchase the book (recommended),
 * visit    http://www.davidflanagan.com/javaexamples2.
 */
        
    // ================================================================
    public void copy()
    // ================================================================
    {
        String s = m_txtArea.getSelectedText();
        StringSelection ss = new StringSelection(s);
        this.getToolkit().getSystemClipboard().setContents(ss, this);
    }

    // ================================================================
    public void copyAll()
    // ================================================================
    {
        String s = m_txtArea.getText();
        StringSelection ss = new StringSelection(s);
        this.getToolkit().getSystemClipboard().setContents(ss, this);
    }    

    // ================================================================
    public void pasteAll()
    // ================================================================
    {
        // Get the contents of the clipboard    
        Clipboard clip  = this.getToolkit().getSystemClipboard();
        Transferable tx = clip.getContents(this);
        
        try{
            // Find out what kind of data is on the clipboard
            if( tx.isDataFlavorSupported(DataFlavor.stringFlavor) ){
                // Only use TEXT data
                m_txtArea.setText( (String) tx.getTransferData(DataFlavor.stringFlavor) );
            }
        }
        catch(Exception e){
            this.getToolkit().beep();    
        }            
    }

    // ================================================================
    public void paste()
    // ================================================================
    {
        // Get the contents of the clipboard    
        Clipboard clip  = this.getToolkit().getSystemClipboard();
        Transferable tx = clip.getContents(this);

        try{
            if (tx.isDataFlavorSupported(DataFlavor.stringFlavor)){
                // Only use TEXT data
                m_txtArea.replaceSelection( (String) tx.getTransferData(DataFlavor.stringFlavor) );
            }
        }
        catch(Exception e){
            this.getToolkit().beep();    
        }            
    }
}
