const customTables = {
    additionalContacts: ["id", 'client_id', 'telegram', 'facebook', 'instagram', 'tiktok'],
    clientRatings: ['rating_id', 'client_id', 'rating', 'comment', 'rating_date'],
    clients: ['id', 'first_name', "last_name", "gender", "sex", "sexual_orientation", "location", 'registration_number', 
    'registered_on', 'age', 'height', 'width', 'zodiac_sign', 'description', 'has_declined_service'],
    complaints: ['complaint_id', 'complainant_id', 'complainee_id', 'date', 'text', 'complaint_status'],
    coupleArchive: ['couple_archive_id', 'first_client_id', 'second_client_id', 'couple_created_on', 'additional_info', 'archived_on'],
    invitations: ['invitation_id', 'inviter_id', 'invitee_id', 'location', 'date_of_meeting', 'created_on', 'active_to', 'is_accepted'],
    meetingReview: ['id', 'inviter_score', 'inviter_review', 'invitee_score', 'invitee_review', 'meeting_id'],
    meetings: ['meeting_id', 'date', 'inviter_id', 'invitee_id', 'location', 'result'],
    meetingVisit: ['id', 'client_id', 'meeting_id', 'visited'],
    partnerRequirements: ['requirement_id', 'gender', 'sex', 'min_age', 'max_age', 'min_height', 'max_height', 'min_weight', 'max_weight', 'zodiac_sign', 'location', 'client_id'],
    phoneNumbers: ['id', 'phone_number', 'additional_contacts_id']
};

CodeMirror.hint.sql.keywords = {
    "SELECT": true, "FROM": true, "WHERE": true, "INSERT": true, 
    "UPDATE": true, "DELETE": true, "ORDER BY": true, "GROUP BY": true,
    "INNER JOIN": true, "LEFT JOIN": true, "RIGHT JOIN": true, "LIMIT": true,
    "DISTINCT": true, "COUNT": true, "SUM": true, "AVG": true, "MIN": true, "MAX": true,
    "CREATE TABLE": true, "DROP TABLE": true, "ALTER TABLE": true,
};

window.initCodeMirror = function (editorId, mode, theme, initialValue, dotNetObject) {
    var editorElement = document.getElementById(editorId);
    if (editorElement) {
        var editor = CodeMirror.fromTextArea(editorElement, {
            lineNumbers: true,
            mode: mode || "",
            theme: theme || "default",
            autoCloseBrackets: true,
            extraKeys: {
                "Ctrl-Space": "autocomplete"
            }
        });

        if (initialValue) {
            editor.setValue(initialValue);
        }

        editor.on("change", function (instance) {
            const value = instance.getValue();
            dotNetObject.invokeMethodAsync('UpdateValue', value);
        });
        
        editor.on("inputRead", function(cm) {
            if (cm.state.completionActive) return;
            CodeMirror.showHint(cm, CodeMirror.hint.sql, {
                completeSingle: false, 
                tables: customTables, 
            });
        });
        
        editor.on("keyup", (cm, event) => {
          const key = event.key;
          if (key === "." || key === " " || /^[a-zA-Z0-9_]+$/.test(key)) {
            CodeMirror.showHint(cm, CodeMirror.hint.sql, {
              completeSingle: false,
              tables: customTables,
            });
          }
        });            
    }
};
